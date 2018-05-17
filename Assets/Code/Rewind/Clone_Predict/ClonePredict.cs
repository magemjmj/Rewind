using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ClonePredict : MonoBehaviour
{
    public Clone m_player;

    Inputs m_last_inputs;

    private uint m_simulate_frame_count;
    private uint m_simulate_start_frame;
    private uint m_simulate_end_frame;

    private uint? m_mismatch_frame;

    private void Start()
    {
        Physics.autoSimulation = false;

        m_simulate_frame_count = 0;
        m_simulate_start_frame = 0;
        m_simulate_end_frame = 0;
        m_mismatch_frame = null;

        Application.logMessageReceived += HandleLog;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Determine())
            RollBack();

        Simulate();
    }


    private bool Determine()
    {
        bool bRet = false;

        // Send
        while (m_player.m_send_input_buffer.Count > 0)
        {
            Inputs send_input = m_player.m_send_input_buffer[0];

            //Debug.Log("Send : " + send_input.frame + " " + send_input.left + " " + send_input.right + " " + send_input.up + " " + send_input.down + " " + send_input.jump);

                byte[] arr = send_input.GetBytes();
            SocketIOManager.GetManager().Emit("sendinput", arr);

            m_player.m_send_input_buffer.RemoveAt(0);
        }


        m_simulate_start_frame = m_player.m_input_start_frame;
        m_simulate_end_frame = m_player.m_input_end_frame;

        //Debug.Log("Plan : " + m_simulate_start_frame + " " + m_simulate_end_frame);


        while (m_player.m_receive_input_buffer.Count > 0)
        {
            Inputs receive_input = m_player.m_receive_input_buffer[0];

            Debug.Log("Receive : " + receive_input.frame + " " + receive_input.left + " " + receive_input.right + " " + receive_input.up + " " + receive_input.down + " " + receive_input.jump);

            if (receive_input.frame < m_simulate_start_frame)
            {
                Inputs previous_input = m_player.GetInput(receive_input.frame);

                if (receive_input != previous_input)
                {
                    if (m_mismatch_frame == null || receive_input.frame < m_mismatch_frame)
                    {
                        m_mismatch_frame = receive_input.frame;
                    }

                    bRet = true;
                }
            }

            m_player.SetInput(receive_input);

            m_player.m_receive_input_buffer.RemoveAt(0);

        }

        return bRet;
    }

    private void RollBack()
    {
        if (m_mismatch_frame != null)
        {
            m_simulate_start_frame = (uint)m_mismatch_frame;

            Debug.Log("RollBack = " + (m_simulate_start_frame - 1));
            m_player.RestorePhyStat(m_simulate_start_frame - 1);

            m_mismatch_frame = null;
        }

    }


    private void Simulate()
    {
        Simulate(m_simulate_start_frame, m_simulate_end_frame);
    }

    private void Simulate(uint start_frame, uint end_frame)
    {
        for (uint frame = start_frame; frame < end_frame; ++frame)
        {
            Inputs inputs = m_player.GetInput(frame);

            if (inputs.frame != frame)
            {
                // Predict Inputs
                inputs = m_last_inputs;
                Debug.Log("Predict : " +
                    inputs.left + " " + inputs.right + " " + inputs.up + " " + inputs.down + " " + inputs.jump
                    );

                m_player.SetInput(frame, inputs);
            }

            m_player.ApplyForce(inputs);

            Physics.Simulate(Time.fixedDeltaTime);

            m_player.SetPhyStat(frame);

            // deterministic
            m_player.RestorePhyStat(frame);

            if (m_last_inputs != inputs)
            {
                Debug.Log("!Simulate : " +
                    frame + " " +
                    m_player.GetComponent<Rigidbody>().position.x + " " +
                    m_player.GetComponent<Rigidbody>().position.y + " " +
                    m_player.GetComponent<Rigidbody>().position.z + " " +
                    inputs.left + " " + inputs.right + " " + inputs.up + " " + inputs.down + " " + inputs.jump
                    );
            } else
            {
                Debug.Log("Simulate : " +
                    frame + " " +
                    m_player.GetComponent<Rigidbody>().position.x + " " +
                    m_player.GetComponent<Rigidbody>().position.y + " " +
                    m_player.GetComponent<Rigidbody>().position.z + " " +
                    inputs.left + " " + inputs.right + " " + inputs.up + " " + inputs.down + " " + inputs.jump
                    );
            }

            m_simulate_frame_count++;
            m_last_inputs = inputs;
        }

        /*
        if (start_frame < end_frame)
            Debug.Log("Simulate Count = " + start_frame + " - " + end_frame);
        */
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logfile;

        if (m_player.m_mode == Clone.CloneMode.Player)
            logfile = "client_log.txt";
        else
            logfile = "remote_log.txt";
        StreamWriter sw = new StreamWriter(logfile.ToString(), true);
        sw.WriteLine(logString);
        sw.Close();
    }


}
