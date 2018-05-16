using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class physics : MonoBehaviour {

    public player m_player;
    public remote m_remote;

    private uint? m_mismatch_frame;

    private uint m_simulate_start_frame;
    private uint m_simulate_end_frame;

    private void Start()
    {
        this.m_mismatch_frame = null;

        Physics.autoSimulation = false;

        m_simulate_start_frame = 0;
        m_simulate_end_frame = 0;
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
                Debug.Log("Determine Mismatch = " + receive_input + " - " + prediction_input);

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
            uint rewind_frame = (uint)m_mismatch_frame - 1;

            m_player.RollBackPhyStat(rewind_frame);
            m_remote.RollBackPhyStat(rewind_frame);

            m_simulate_start_frame = rewind_frame;

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
            m_player.ApplyForce(inputs);
            m_player.SetPhyStat(frame);

            inputs = m_remote.GetInput(frame);
            m_remote.ApplyForce(inputs);
            m_remote.SetPhyStat(frame);

            Physics.Simulate(Time.fixedDeltaTime);
        }

        Debug.Log("Simulate Count = " + start_frame + " - " + end_frame);
    }


}
