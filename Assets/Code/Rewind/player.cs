using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : PhySyncBase
{
    public float move_force = 10.0f;
    public float jump_force = 100.0f;

    private void Start()
    {
    }

    private void Update()
    {
        Inputs inputs;

        inputs.frame = 0;
        inputs.up = Input.GetKey(KeyCode.W);
        inputs.down = Input.GetKey(KeyCode.S);
        inputs.left = Input.GetKey(KeyCode.A);
        inputs.right = Input.GetKey(KeyCode.D);
        inputs.jump = Input.GetKey(KeyCode.Space);

        ProcessInput(inputs);
        SendInputToServer();
    }

    public void ApplyForce(Inputs inputs)
    {
        Vector3 finalforce = Vector3.zero;
        if (inputs.up) finalforce += Vector3.forward * move_force;
        if (inputs.down) finalforce += Vector3.back * move_force;
        if (inputs.left) finalforce += Vector3.left * move_force;
        if (inputs.right) finalforce += Vector3.right * move_force;
        if (inputs.jump) finalforce += Vector3.up * jump_force;

        m_rigid.AddForce(finalforce);

    }

    protected void SendInputToServer()
    {
        base.m_send_end_frame = base.m_input_end_frame;
        for (uint frame = base.m_send_start_frame; frame < base.m_send_end_frame; ++frame)
        {
            Inputs inputs = GetInput(frame);
            byte[] arr = inputs.GetBytes();
            SocketIOManager.GetManager().Emit("sendinput", arr);
        }
        base.m_send_start_frame = base.m_send_end_frame;
    }


}
