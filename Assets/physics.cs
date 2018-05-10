﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class physics : MonoBehaviour {

    public player m_player;
    public remote m_remote;

    private uint? m_mismatch_frame;

    private void Start()
    {
        this.m_mismatch_frame = null;

        Physics.autoSimulation = false;
    }

    // Update is called once per frame
    private void Update()
    {
        Determine();
        Rewind();
        Simulate();
    }

    private void Determine()
    {
        while (m_remote.m_receive_input_buffer.Count > 0)
        {
            Inputs receive_input = m_remote.m_receive_input_buffer[0];

            Inputs prediction_input = m_remote.GetInput(receive_input.frame);

            if (receive_input != prediction_input)
            {
                if (m_mismatch_frame == null)
                {
                    m_mismatch_frame = receive_input.frame;
                } else

                if (m_mismatch_frame > receive_input.frame)
                {
                    m_mismatch_frame = receive_input.frame;
                }

                m_remote.SetInput(receive_input);
            }

            m_remote.m_receive_input_buffer.RemoveAt(0);
        }
    }

    private void Rewind()
    {
        if (m_mismatch_frame != null)
        {
            uint rewind_frame = (uint)m_mismatch_frame;

            m_remote.RewindPhyStat(rewind_frame);
            m_player.m_input_start_frame = rewind_frame;

            m_mismatch_frame = null;
        }
    }

    private void Simulate()
    {
        Simulate(m_player.m_input_start_frame, m_player.m_input_end_frame);
    }

    private void Simulate(uint start_frame, uint end_frame)
    {
        for (uint frame = start_frame; frame < end_frame; ++frame)
        {
            Inputs inputs = m_player.GetInput(frame);
            m_player.ApplyForce(inputs);
            m_player.SetPhyStat(frame);

            inputs = m_remote.GetInput(frame);
            m_remote.ApplyForce(inputs);
            m_remote.SetPhyStat(frame);

            Physics.Simulate(Time.fixedDeltaTime);
        }

        Debug.Log("Simulate Count = " + (end_frame - start_frame));
    }


}
