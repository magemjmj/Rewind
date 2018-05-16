using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remote : PhySyncBase
{
    public float move_force = 10.0f;
    public float jump_force = 100.0f;

    public List<Inputs> m_send_input_buffer = new List<Inputs>();
    public List<Inputs> m_receive_input_buffer = new List<Inputs>();

    // Use this for initialization
    void Start ()
    {	
	}
	
	// Update is called once per frame
	void Update ()
    {
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

    public void ReceiveInputFromServer(Inputs receive_input)
    {
        m_receive_input_buffer.Add(receive_input);
    }
}
