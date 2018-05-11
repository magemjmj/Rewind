using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TCB3f
{
	public float m_time = 0.0f;
    public float m_tension = 0.0f;
    public float m_bias = 0.0f;
    public float m_continuity = 0.0f;

    public Vector3 m_prev, m_cur, m_next;
    public Vector3 m_tangent_source;
    public Vector3 m_tangent_dest;

    public TCB3f() {}
    public TCB3f(float time) { m_time = time;  }
    public TCB3f(float time, Vector3 cur) { m_time = time; m_cur = cur; }

    public void SetPoint(Vector3 pos) { m_cur = pos; }
    public void SetTime(float time) { m_time = time; }

    public void CalcTangent()
    {
        float tension1 = 1.0f - m_tension;
        float bias1 = 1.0f + m_bias;
        float bias2 = 1.0f - m_bias;
        float continuity1 = 1.0f - m_continuity;
        float continuity2 = 1.0f + m_continuity;

        float parm1 = (tension1 * bias1 * continuity1) / 2.0f;
        float parm2 = (tension1 * bias2 * continuity2) / 2.0f;
        float parm3 = (tension1 * bias1 * continuity2) / 2.0f;
        float parm4 = (tension1 * bias2 * continuity1) / 2.0f;

        m_tangent_source = parm1 * (m_cur - m_prev) + parm2 * (m_next - m_cur);
        m_tangent_dest = parm3 * (m_cur - m_prev) + parm4 * (m_next - m_cur);
    }
}

public class Curve3f
{
    private List<TCB3f> m_tcbvector;
    public Curve3f()
    {
        m_tcbvector = new List<TCB3f>();
    }

    public void Clear() { m_tcbvector.Clear(); }
    public void AddTCB(TCB3f p) { m_tcbvector.Add(p); }
    public int size() { return m_tcbvector.Count; }
	public TCB3f GetTCB(int index) { return m_tcbvector[index]; }
    public void SetTCB(int index, TCB3f p) { m_tcbvector[index] = p; }

    public void CalcCurve()
    {
        if (m_tcbvector.Count == 0) return;

        m_tcbvector.Sort(delegate (TCB3f a, TCB3f b)
        {
            return a.m_time < b.m_time ? -1 : a.m_time > b.m_time ? 1 : 0;
        });

        for (int i = 0; i < m_tcbvector.Count; ++i)
        {
            TCB3f tcb_curr = m_tcbvector[i];

            if (i == 0)
            {
                tcb_curr.m_prev = tcb_curr.m_cur;
            }

            if (i > 0)
            {
                TCB3f tcb_prev = m_tcbvector[i - 1];
                tcb_curr.m_prev = tcb_prev.m_cur;
            }

            if (i + 1 < m_tcbvector.Count)
            {
                TCB3f tcb_next = m_tcbvector[i + 1];
                tcb_curr.m_next = tcb_next.m_cur;
            }

            if (i + 1 == m_tcbvector.Count)
            {
                tcb_curr.m_next = tcb_curr.m_cur;
            }

            tcb_curr.CalcTangent();
            m_tcbvector[i] = tcb_curr;
        }
    }

    public bool GetCurve(float time, ref Vector3 pPos)
    {
        if (m_tcbvector.Count < 2) return false;

        TCB3f tcb_front = m_tcbvector[0];
        TCB3f tcb_back = m_tcbvector[m_tcbvector.Count - 1];

        if (time <= tcb_front.m_time)
        {
            pPos = tcb_front.m_cur;
            return true;
        }

        if (time >= tcb_back.m_time)
        {
            pPos = tcb_back.m_cur;
            return true;
        }

        int index = Utility.UpperBinarySearch(m_tcbvector, time, delegate (TCB3f a, float b)
        {
            return a.m_time < b ? -1 : a.m_time > b ? 1 : 0;
        });

        if (index != -1)
        {
            TCB3f tcb_prev = m_tcbvector[index - 1];
            TCB3f tcb_cur = m_tcbvector[index];

            float prev_time = tcb_prev.m_time;
            float cur_time = tcb_cur.m_time;
            // if (prev_time == time) return tcb_prev.m_cur;

            float t = (time - prev_time) / (cur_time - prev_time);

            Vector3 p0 = tcb_prev.m_cur;
            Vector3 p1 = tcb_cur.m_cur;
            Vector3 m0 = tcb_prev.m_tangent_dest;
            Vector3 m1 = tcb_cur.m_tangent_source;

            float t2 = t * t;
            float t3 = t2 * t;

            pPos = (2.0f * t3 - 3.0f * t2 + 1.0f) * p0 +
                   (t3 - 2.0f * t2 + t) * m0 +
                   (t3 - t2) * m1 +
                   (-2.0f * t3 + 3.0f * t2) * p1;

            return true;
        }

        return false;
    }

    public bool GetTangent(float time, ref Vector3 pPos)
    {
        if (m_tcbvector.Count < 2) return false;

        TCB3f tcb_front = m_tcbvector[0];
        TCB3f tcb_back = m_tcbvector[m_tcbvector.Count - 1];

        if (time <= tcb_front.m_time)
		{
			pPos = tcb_front.m_tangent_source;
			return true;
		}

		if (time >= tcb_back.m_time)
		{
			pPos = tcb_back.m_tangent_source;
			return true;
		}

        int index = Utility.UpperBinarySearch(m_tcbvector, time, delegate (TCB3f a, float b)
        {
            return a.m_time < b ? -1 : a.m_time > b ? 1 : 0;
        });

        if (index != -1)
        {
            TCB3f tcb_prev = m_tcbvector[index - 1];
            TCB3f tcb_cur = m_tcbvector[index];

            float prev_time = tcb_prev.m_time;
			float cur_time = tcb_cur.m_time;
			// if (prev_time == time) return tcb_prev.m_cur;

			float t = (time - prev_time) / (cur_time - prev_time);

			Vector3 p0 = tcb_prev.m_cur;
			Vector3 p1 = tcb_cur.m_cur;
			Vector3 m0 = tcb_prev.m_tangent_dest;
			Vector3 m1 = tcb_cur.m_tangent_source;

			float t2 = t * t;

			pPos = (6.0f * t2 - 6.0f * t) * p0 +
				   (3.0f * t2 - 4.0f * t + 1.0f) * m0 +
				   (3.0f * t2 - 2.0f * t) * m1 +
				   (-6.0f * t2 + 6.0f * t) * p1;

			return true;
		}

		return false;
	}

}

