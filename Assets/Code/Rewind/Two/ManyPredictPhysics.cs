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
        m_mismatch_frame = null;

        foreach (PlayerSync player in m_players)
        {
            SendLocalInput(player);
        }

        foreach (PlayerSync player in m_players)
        {
            DetermineReceiveInput(player);
        }

        if (m_mismatch_frame != null)
        {
            foreach (PlayerSync player in m_players)
            {
                RollBack(player);
            }
        }

        Simulate();
    }


    private void SendLocalInput(PlayerSync player)
    {
        if (player.PlayerSyncType != PlayerSync.ePlayerSyncType.Local)
            return;

        // Send
        while (player.m_send_input_buffer.Count > 0)
        {
            Inputs send_input = player.m_send_input_buffer[0];

            //Debug.Log("Send : " + send_input.frame + " " + send_input.left + " " + send_input.right + " " + send_input.up + " " + send_input.down + " " + send_input.jump);

            byte[] arr = send_input.GetBytes();
            SocketIOManager.GetManager().Emit("sendinput", player.PlayerID, arr);

            player.m_send_input_buffer.RemoveAt(0);
        }


        m_simulate_start_frame = player.InputStartFrame;
        m_simulate_end_frame = player.InputEndFrame;

        //Debug.Log("Plan : " + m_simulate_start_frame + " " + m_simulate_end_frame);;
    }

    private void DetermineReceiveInput(PlayerSync player)
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

    private void RollBack(PlayerSync player)
    {
        m_simulate_start_frame = (uint)m_mismatch_frame;

        Debug.Log("RollBack = " + (m_simulate_start_frame - 1));
        player.RestorePhyStat(m_simulate_start_frame - 1);
    }


    private void Simulate()
    {
        if (m_simulate_start_frame != m_simulate_end_frame)
        {
            foreach (PlayerSync player in m_players)
            {
                Simulate(player, m_simulate_start_frame, m_simulate_end_frame);
            }

            Physics.Simulate(Time.fixedDeltaTime);

            foreach (PlayerSync player in m_players)
            {
                Deterministic(player, m_simulate_start_frame, m_simulate_end_frame);
            }
        }
    }

    private void Simulate(PlayerSync player, uint start_frame, uint end_frame)
    {
        for (uint frame = start_frame; frame < end_frame; ++frame)
        {
            Inputs inputs = player.GetInput(frame);

            if (inputs.frame != frame)
            {
                // Predict Inputs
                inputs = player.LastSimulateInput;
                /*
                Debug.Log("Predict : " +
                    inputs.left + " " + inputs.right + " " + inputs.up + " " + inputs.down + " " + inputs.jump
                    );
                */
                player.SetInput(frame, inputs);
            }

            player.ApplyForce(inputs);

        }
    }

    private void Deterministic(PlayerSync player, uint start_frame, uint end_frame)
    {
        for (uint frame = start_frame; frame < end_frame; ++frame)
        {
            player.SetPhyStat(frame);
            // deterministic
            player.RestorePhyStat(frame);
        }

        /*
        if (start_frame < end_frame)
            Debug.Log("Simulate Count = " + start_frame + " - " + end_frame);
        */
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
