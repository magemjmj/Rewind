using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSync : PhySyncBase
{
    public enum ePlayerSyncType
    {
        Local = 0,
        Remote,
    }

    public ePlayerSyncType m_playerSyncType;
    public ePlayerSyncType PlayerSyncType { get { return m_playerSyncType; } }

    public uint m_player_id;
    public uint PlayerID { get { return m_player_id;  } set { m_player_id = value; } }

    public float move_force = 10.0f;
    public float jump_force = 100.0f;

    public List<Inputs> m_send_input_buffer = new List<Inputs>();
    public List<Inputs> m_receive_input_buffer = new List<Inputs>();

    private Inputs m_last_send_inputs;

    protected Inputs m_last_simulate_inputs;
    public Inputs LastSimulateInput { get { return m_last_simulate_inputs; } set { m_last_simulate_inputs = value; } }

    private void Start()
    {
    }

    private void Update()
    {
        if (m_playerSyncType == ePlayerSyncType.Local)
            PlayerUpdate();
        else
            RemoteUpdate();
    }


    private void PlayerUpdate()
    {
        this.m_timer += Time.deltaTime;
        this.m_input_start_frame = this.m_input_process_frame;
        /*
        FlatBufferBuilder builder = new FlatBufferBuilder(1);
        Inputs.StartInputs(builder);
        Inputs.AddFrame(builder, this.m_input_start_frame);
        Inputs.AddUp(builder, Input.GetKey(KeyCode.W));
        Inputs.AddDown(builder, Input.GetKey(KeyCode.S));
        Inputs.AddLeft(builder, Input.GetKey(KeyCode.A));
        Inputs.AddRight(builder, Input.GetKey(KeyCode.D));
        Inputs.AddJump(builder, Input.GetKey(KeyCode.Space));

        var offset = Inputs.EndInputs(builder);
        Inputs.FinishInputsBuffer(builder, offset);
        */

        Inputs inputs = new Inputs();
        inputs.frame = this.m_input_start_frame;
        inputs.up = Input.GetKey(KeyCode.W);
        inputs.down = Input.GetKey(KeyCode.S);
        inputs.left = Input.GetKey(KeyCode.A);
        inputs.right = Input.GetKey(KeyCode.D);
        inputs.jump = Input.GetKey(KeyCode.Space);

        while (this.m_timer >= Time.fixedDeltaTime)
        {
            this.m_timer -= Time.fixedDeltaTime;
            this.m_input_process_frame++;
        }

        this.m_input_end_frame = this.m_input_process_frame;

        if (this.m_input_start_frame != this.m_input_end_frame)
        {
            if (inputs != this.m_last_send_inputs)
            {
                SendInputToServer(inputs);
                ReceiveInputFromServer(inputs);

                this.m_last_send_inputs = inputs;
            }
        }
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
