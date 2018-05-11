using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;


[StructLayout(LayoutKind.Explicit)]
public class UnionType
{
    public enum eUnionType
    {
        eNone,
        eBool,
        eInt,
        eFloat,
    }

    [FieldOffset(0)] eUnionType m_Type = eUnionType.eNone;
    [FieldOffset(4)] bool m_bValue;
    [FieldOffset(4)] int m_iValue;
    [FieldOffset(4)] float m_fValue;
    //[FieldOffset(4)] string m_sValue;
    //[FieldOffset(4)] object m_oValue;

    public UnionType() {}
    public UnionType(bool bBool) { m_Type = eUnionType.eBool; m_bValue = bBool; }
    public UnionType(int iValue) { m_Type = eUnionType.eInt; m_iValue = iValue; }
    public UnionType(float fValue) { m_Type = eUnionType.eFloat; m_fValue = fValue; }
    //public UnionType(string sValue) { m_Type = eUnionType.eString; m_sValue = sValue; }
    //public UnionType(object oValue) { m_Type = eUnionType.eObject; m_oValue = oValue; }

    public eUnionType GetUnionType() { return m_Type; }
    public bool GetBool() { Assert.IsTrue(GetUnionType() == eUnionType.eBool); return m_bValue; }
    public int GetInt() { Assert.IsTrue(GetUnionType() == eUnionType.eInt); return m_iValue; }
    public float GetFloat() { Assert.IsTrue(GetUnionType() == eUnionType.eFloat); return m_fValue; }
    //public string GetString() { Assert.IsTrue(GetUnionType() == eUnionType.eString); return m_sValue; }
    //public object GetObject() { Assert.IsTrue(GetUnionType() == eUnionType.eObject); return m_oValue; }

    public void SetBool(bool bBool) { m_Type = eUnionType.eBool; m_bValue = bBool; }
    public void SetInt(int iValue) { m_Type = eUnionType.eInt; m_iValue = iValue; }
    public void SetFloat(float fValue) { m_Type = eUnionType.eFloat; m_fValue = fValue; }
    //public void SetString(string sValue) { m_Type = eUnionType.eString; m_sValue = sValue; }
    //public void SetObject(object oValue) { m_Type = eUnionType.eObject; m_oValue = oValue; }

    public static bool operator ==(UnionType lhs, UnionType rhs)
    {
        switch (lhs.GetUnionType())
        {
            case eUnionType.eBool:
                return lhs.GetBool() == rhs.GetBool();
            case eUnionType.eInt:
                return lhs.GetInt() == rhs.GetInt();
            case eUnionType.eFloat:
                return lhs.GetFloat() == rhs.GetFloat();
            default: return false;
        }
    }

    public static bool operator !=(UnionType lhs, UnionType rhs)
    {
        switch (lhs.GetUnionType())
        {
            case eUnionType.eBool:
                return lhs.GetBool() != rhs.GetBool();
            case eUnionType.eInt:
                return lhs.GetInt() != rhs.GetInt();
            case eUnionType.eFloat:
                return lhs.GetFloat() != rhs.GetFloat();
            default: return false;
        }
    }

    public static bool operator <(UnionType lhs, UnionType rhs)
    {
		switch (lhs.GetUnionType())
		{
            case eUnionType.eBool:
                return false;
            case eUnionType.eInt:
                return lhs.GetInt() < rhs.GetInt();
            case eUnionType.eFloat:
                return lhs.GetFloat() < rhs.GetFloat();
            default: return false;
        }
	}

    public static bool operator >(UnionType lhs, UnionType rhs)
    {
        switch (lhs.GetUnionType())
        {
            case eUnionType.eBool:
                return false;
            case eUnionType.eInt:
                return lhs.GetInt() > rhs.GetInt();
            case eUnionType.eFloat:
                return lhs.GetFloat() > rhs.GetFloat();
            default: return false;
        }
    }

    public static bool operator <=(UnionType lhs, UnionType rhs)
    {
        switch (lhs.GetUnionType())
        {
            case eUnionType.eBool:
                return false;
            case eUnionType.eInt:
                return lhs.GetInt() <= rhs.GetInt();
            case eUnionType.eFloat:
                return lhs.GetFloat() <= rhs.GetFloat();
            default: return false;
        }
    }

    public static bool operator >=(UnionType lhs, UnionType rhs)
    {
        switch (lhs.GetUnionType())
        {
            case eUnionType.eBool:
                return false;
            case eUnionType.eInt:
                return lhs.GetInt() >= rhs.GetInt();
            case eUnionType.eFloat:
                return lhs.GetFloat() >= rhs.GetFloat();
            default: return false;
        }
    }

    public override bool Equals(object obj)
    {
        return this == (UnionType)obj;
    }

    public override int GetHashCode()
    {
        return m_iValue;
    }
}

public class BlackBoard
{
    protected Dictionary<string, UnionType> m_unions;
    protected Dictionary<string, object> m_objects;

    public void SetUnion(string key, UnionType value) { m_unions[key] = value; }
    public bool GetUnion(string key, out UnionType pvalue)
    {
        if (m_unions.TryGetValue(key, out pvalue))
        {
            return true;
        }

        return false;
    }

    public void SetObject(string key, object value) { m_objects[key] = value; }
    public bool GetObject(string key, out object pvalue)
    {
        if (m_objects.TryGetValue(key, out pvalue))
        {
            return true;
        }

        return false;
    }
}