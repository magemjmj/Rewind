using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhySyncBase : MonoBehaviour {

    public Inputs[] m_input_buffer = new Inputs[1024];
    public PhyStat[] m_phy_buffer = new PhyStat[1024];

    protected Rigidbody m_rigid;

    protected float m_timer;

    public uint m_input_start_frame;
    public uint m_input_process_frame;
    public uint m_input_end_frame;

    public uint m_phy_start_frame;
    public uint m_phy_process_frame;
    public uint m_phy_end_frame;

    private void Awake()
    {
        m_rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        this.m_timer = 0.0f;

        this.m_input_start_frame = 0;
        this.m_input_process_frame = 0;
        this.m_input_end_frame = 0;

        this.m_phy_start_frame = 0;
        this.m_phy_process_frame = 0;
        this.m_phy_end_frame = 0;
    }

    public Inputs GetInput(uint frame)
    {
        return m_input_buffer[frame % 1024];
    }

    public void SetInput(Inputs inputs)
    {
        m_input_buffer[inputs.frame % 1024] = inputs;
    }


    public PhyStat GetPhyStat(uint frame)
    {
        return m_phy_buffer[frame % 1024];
    }

    public void SetPhyStat(uint frame)
    {
        PhyStat stat;
        stat.position = m_rigid.position;
        stat.rotation = m_rigid.rotation;
        stat.velocity = m_rigid.velocity;
        stat.angularVelocity = m_rigid.angularVelocity;
        m_phy_buffer[frame % 1024] = stat;
    }

    public void RewindPhyStat(uint frame)
    {
        PhyStat stat = m_phy_buffer[frame % 1024];
        m_rigid.position = stat.position;
        m_rigid.rotation = stat.rotation;
        m_rigid.velocity = stat.velocity;
        m_rigid.angularVelocity = stat.angularVelocity;
    }

    protected void ProcessInput(Inputs inputs)
    {
        this.m_timer += Time.deltaTime;

        this.m_input_start_frame = this.m_input_process_frame;
        while (this.m_timer >= Time.fixedDeltaTime)
        {
            this.m_timer -= Time.fixedDeltaTime;

            inputs.frame = this.m_input_process_frame;
            SetInput(inputs);

            this.m_input_process_frame++;
        }
        this.m_input_end_frame = this.m_input_process_frame;
    }


}
