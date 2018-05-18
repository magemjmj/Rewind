using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ClonePhysics : MonoBehaviour
{
    public PlayerSync m_player;

    Inputs m_prev_inputs;

    private uint m_simulate_frame_count;
    private uint m_simulate_start_frame;
    private uint m_simulate_end_frame;

    private void Start()
    {
        Physics.autoSimulation = false;

        m_simulate_frame_count = 0;
        m_simulate_start_frame = 0;
        m_simulate_end_frame = 0;

        Application.logMessageReceived += HandleLog;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Determine())
            Simulate();
    }

    private bool Determine()
    {
        bool bRet = false;

        while (m_player.m_send_input_buffer.Count > 0)
        {
            Inputs send_input = m_player.m_send_input_buffer[0];

            byte[] arr = send_input.GetBytes();
            SocketIOManager.GetManager().Emit("sendinput", arr);

            m_player.m_send_input_buffer.RemoveAt(0);
        }

        int count = m_player.m_receive_input_buffer.Count;
        if (count > 0)
        {
            m_simulate_start_frame = m_player.m_receive_input_buffer[0].frame;
            m_simulate_end_frame = m_player.m_receive_input_buffer[count - 1].frame + 1;
            bRet = true;
        }

        while (m_player.m_receive_input_buffer.Count > 0)
        {
            Inputs receive_input = m_player.m_receive_input_buffer[0];
            m_player.SetInput(receive_input);
            m_player.m_receive_input_buffer.RemoveAt(0);

        }

        return bRet;
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
            m_player.ApplyForce(inputs);
            m_player.SetPhyStat(frame);

            m_simulate_frame_count++;

            Physics.Simulate(Time.fixedDeltaTime);

            if (m_prev_inputs != inputs)
            {
                Debug.Log("Simulate : " + inputs.frame + " " +
                    m_simulate_frame_count + " " +
                    m_player.transform.position.x + " " +
                    m_player.transform.position.y + " " +
                    m_player.transform.position.z
                    );
            }


            m_prev_inputs = inputs;
        }

        /*
        if (start_frame < end_frame)
            Debug.Log("Simulate Count = " + start_frame + " - " + end_frame);
        */
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logfile;

        if (m_player.PlayerSyncType == PlayerSync.ePlayerSyncType.Local)
            logfile = "client_log.txt";
        else
            logfile = "remote_log.txt";
        StreamWriter sw = new StreamWriter(logfile.ToString(), true);
        sw.WriteLine(logString);
        sw.Close();
    }


}
