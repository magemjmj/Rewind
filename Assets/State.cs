using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Inputs
{
    public uint frame;
    public bool up;
    public bool down;
    public bool left;
    public bool right;
    public bool jump;

    public static bool operator ==(Inputs l, Inputs r)
    {
        return l.Equals(r);
    }

    public static bool operator !=(Inputs l, Inputs r)
    {
        return !l.Equals(r);
    }

    public bool Equals(Inputs r)
    {
        if (frame != r.frame ||
            up != r.up ||
            down != r.down ||
            left != r.left ||
            right != r.right ||
            jump != r.jump)
            return false;

        return true;
    }

}



public struct PhyStat
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
}
