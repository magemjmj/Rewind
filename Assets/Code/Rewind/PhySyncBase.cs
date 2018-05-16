using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhySyncBase : MonoBehaviour {

    public const uint MAX_BUFFER = 1024;

    public Inputs[] m_input_buffer = new Inputs[MAX_BUFFER];
    public PhyStat[] m_phy_buffer = new PhyStat[MAX_BUFFER];

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
        return m_input_buffer[frame % MAX_BUFFER];
    }

    public void SetInput(Inputs inputs)
    {
        m_input_buffer[inputs.frame % MAX_BUFFER] = inputs;
    }


    public void SetPhyStat(uint frame)
    {
        PhyStat stat;
        stat.position = m_rigid.position;
        stat.rotation = m_rigid.rotation;
        stat.velocity = m_rigid.velocity;
        stat.angularVelocity = m_rigid.angularVelocity;
        m_phy_buffer[frame % MAX_BUFFER] = stat;

        /*
        Debug.Log("SetPhyStat : Pos " + stat.position.x + " " + stat.position.y + " " + stat.position.z);
        Debug.Log("SetPhyStat : Rot " + stat.rotation.x + " " + stat.rotation.y + " " + stat.rotation.z + " " + stat.rotation.w);
        Debug.Log("SetPhyStat : Vel " + stat.velocity.x + " " + stat.velocity.y + " " + stat.velocity.z);
        Debug.Log("SetPhyStat : Ang " + stat.angularVelocity.x + " " + stat.angularVelocity.y + " " + stat.angularVelocity.z);
        */

    }

    public void RollBackPhyStat(uint frame)
    {
        PhyStat stat = m_phy_buffer[frame % MAX_BUFFER];
        m_rigid.position = stat.position;
        m_rigid.rotation = stat.rotation;
        m_rigid.velocity = stat.velocity;
        m_rigid.angularVelocity = stat.angularVelocity;
        /*
        Debug.Log("BakPhyStat : Pos " + stat.position.x + " " + stat.position.y + " " + stat.position.z);
        Debug.Log("BakPhyStat : Rot " + stat.rotation.x + " " + stat.rotation.y + " " + stat.rotation.z + " " + stat.rotation.w);
        Debug.Log("BakPhyStat : Vel " + stat.velocity.x + " " + stat.velocity.y + " " + stat.velocity.z);
        Debug.Log("BakPhyStat : Ang " + stat.angularVelocity.x + " " + stat.angularVelocity.y + " " + stat.angularVelocity.z);
        */
    }


}
