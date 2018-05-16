using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone : PhySyncBase
{

    public enum CloneMode
    {
        Player = 0,
        Remote,
    }

    public CloneMode m_mode;

    public float move_force = 10.0f;
    public float jump_force = 100.0f;

    public List<Inputs> m_send_input_buffer = new List<Inputs>();
    public List<Inputs> m_receive_input_buffer = new List<Inputs>();

    private Inputs m_last_inputs;

    private void Start()
    {
    }

    private void Update()
    {
        if (m_mode == CloneMode.Player)
            PlayerUpdate();
        else
            RemoteUpdate();
    }


    private void PlayerUpdate()
    {
        Inputs inputs;

        inputs.frame = 0;
        inputs.up = Input.GetKey(KeyCode.W);
        inputs.down = Input.GetKey(KeyCode.S);
        inputs.left = Input.GetKey(KeyCode.A);
        inputs.right = Input.GetKey(KeyCode.D);
        inputs.jump = Input.GetKey(KeyCode.Space);

        this.m_timer += Time.deltaTime;

        this.m_input_start_frame = this.m_input_process_frame;

        while (this.m_timer >= Time.fixedDeltaTime)
        {
            this.m_timer -= Time.fixedDeltaTime;

            inputs.frame = this.m_input_process_frame;

            if (inputs != m_last_inputs)
            {
                SendInputToServer(inputs);
                ReceiveInputFromServer(inputs);

                m_last_inputs = inputs;
            }

            this.m_input_process_frame++;
        }

        this.m_input_end_frame = this.m_input_process_frame;
    }

    private void RemoteUpdate()
    {
        this.m_timer += Time.deltaTime;

        this.m_input_start_frame = this.m_input_process_frame;

        while (this.m_timer >= Time.fixedDeltaTime)
        {
            this.m_timer -= Time.fixedDeltaTime;
            this.m_input_process_frame++;
        }

        this.m_input_end_frame = this.m_input_process_frame;
    }

    public void ApplyForce(Inputs inputs)
    {
        Vector3 finalforce = Vector3.zero;
        if (inputs.up) finalforce += Vector3.forward * move_force;
        if (inputs.down) finalforce += Vector3.back * move_force;
        if (inputs.left) finalforce += Vector3.left * move_force;
        if (inputs.right) finalforce += Vector3.right * move_force;
        if (inputs.jump) finalforce += Vector3.up * jump_force;

        m_rigid.AddForce(finalforce, ForceMode.Impulse);

    }

    protected void SendInputToServer(Inputs send_input)
    {
        m_send_input_buffer.Add(send_input);
    }

    public void ReceiveInputFromServer(Inputs receive_input)
    {
        m_receive_input_buffer.Add(receive_input);
    }


}
