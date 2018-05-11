using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public struct Inputs
{
    public uint frame;
    //[MarshalAs(UnmanagedType.U4)]
    public bool up;
    //[MarshalAs(UnmanagedType.U1)]
    public bool down;
    //[MarshalAs(UnmanagedType.U1)]
    public bool left;
    //[MarshalAs(UnmanagedType.U1)]
    public bool right;
    //[MarshalAs(UnmanagedType.U1)]
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
        if (/*frame != r.frame ||*/
            up != r.up ||
            down != r.down ||
            left != r.left ||
            right != r.right ||
            jump != r.jump)
            return false;

        return true;
    }

    public byte[] GetBytes()
    {
        int size = Marshal.SizeOf(this);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(this, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);

        return arr;
    }

    public static Inputs FromBytes(byte[] arr)
    {
        Inputs inputs = new Inputs();

        int size = Marshal.SizeOf(inputs);
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.Copy(arr, 0, ptr, size);

        inputs = (Inputs)Marshal.PtrToStructure(ptr, inputs.GetType());
        Marshal.FreeHGlobal(ptr);

        return inputs;
    }

}



public struct PhyStat
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
}
