using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ManyPredictPhysics : MonoBehaviour
{
    public PlayerSync[] m_players;

    private uint m_simulate_start_frame;
    private uint m_simulate_end_frame;

    private uint? m_mismatch_frame;

    private void Start()
    {
        Physics.autoSimulation = false;

        m_simulate_start_frame = 0;
        m_simulate_end_frame = 0;
        m_mismatch_frame = null;

        Application.logMessageReceived += HandleLog;
    }

    // Update is called once per frame
    private void Update()
    {
        SendLocalInput();
        DetermineReceiveInput();
        RollBack();
        Simulate();
    }


    private void SendLocalInput()
    {
        foreach (PlayerSync player in m_players)
        {
            if (player.PlayerSyncType != PlayerSync.ePlayerSyncType.Local)
                continue;

            // Send
            while (player.m_send_input_buffer.Count > 0)
            {
                Inputs send_input = player.m_send_input_buffer[0];

                //Debug.Log("Send : " + send_input.frame + " " + send_input.left + " " + send_input.right + " " + send_input.up + " " + send_input.down + " " + send_input.jump);

                ManySocket.GetManager().SendInputs("sendinput", player.PlayerID, send_input);

                player.m_send_input_buffer.RemoveAt(0);
            }


            m_simulate_start_frame = player.InputStartFrame;
            m_simulate_end_frame = player.InputEndFrame;
        }

        //Debug.Log("Plan : " + m_simulate_start_frame + " " + m_simulate_end_frame);;
    }

    private void DetermineReceiveInput()
    {
        m_mismatch_frame = null;
        
        foreach (PlayerSync player in m_players)
        {
            while (player.m_receive_input_buffer.Count > 0)
            {
                Inputs receive_input = player.m_receive_input_buffer[0];

                Debug.Log("Receive : " + receive_input.frame + " " + receive_input.left + " " + receive_input.right + " " + receive_input.up + " " + receive_input.down + " " + receive_input.jump);

                if (receive_input.frame < m_simulate_start_frame)
                {
                    Inputs previous_input = player.GetInput(receive_input.frame);

                    if (receive_input != previous_input)
                    {
                        if (m_mismatch_frame == null || receive_input.frame < m_mismatch_frame)
                        {
                            m_mismatch_frame = receive_input.frame;
                        }
                    }
                }

                player.SetInput(receive_input);
                player.m_receive_input_buffer.RemoveAt(0);
            }
        }
    }

    private void RollBack()
    {
        if (m_mismatch_frame != null)
        {
            m_simulate_start_frame = (uint)m_mismatch_frame;
            Debug.Log("RollBack = " + (m_simulate_start_frame - 1));

            foreach (PlayerSync player in m_players)
            {
                player.RestorePhyStat(m_simulate_start_frame - 1);
            }
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
            foreach (PlayerSync player in m_players)
            {
                Inputs inputs = player.GetInput(frame);

                if (inputs.frame != frame)
                {
                    // Predict Inputs
                    inputs = player.LastSimulateInput;
                    player.SetInput(frame, inputs);
                }

                Debug.Log("Simulate[" + frame + " " + player.PlayerID + "]" + " " +
                    inputs.left + " " + inputs.right + " " + inputs.up + " " + inputs.down + " " + inputs.jump
                    );

                player.ApplyForce(inputs);
            }
            
            Physics.Simulate(Time.fixedDeltaTime);

            // deterministic
            foreach (PlayerSync player in m_players)
            {
                player.SetPhyStat(frame);
                player.RestorePhyStat(frame);

                Debug.Log("Player[" + player.PlayerID + "]" +
                    player.GetComponent<Rigidbody>().position.x + " " + player.GetComponent<Rigidbody>().position.y + " " + player.GetComponent<Rigidbody>().position.z
                    );
            }
        }
    }


    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logfile;

        logfile = "many_log.txt";

        StreamWriter sw = new StreamWriter(logfile.ToString(), true);
        sw.WriteLine(logString);
        sw.Close();
    }


}
