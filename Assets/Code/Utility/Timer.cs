using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CClock
{
    public CClock() {}
    public CClock(bool bStart)
    {
        if (bStart)
            Start();
    }

    ~CClock() {}

    private float m_StartClock = 0;
    private float m_StopClock = float.MaxValue;
    private float m_StartTime = 0;
    private float m_StopTime = 0;
    private bool m_bRun = false;
    private bool m_bAutoStop = false;
    private float m_fSpeed = 1.0f;
    private float m_CurrentTime = 0;
    private float m_CurrentClock = 0;
    private float m_BeforeClock = 0;

    public void Start()
    {
        m_bRun = true;
        m_CurrentTime = 0;
        m_CurrentClock = m_StartClock;
        m_BeforeClock = m_StartClock;
        m_StartTime = GetCurrentTime();
        m_StopTime = 0;
    }

    public void Start(float time)
    {
        m_bRun = true;
        m_CurrentTime = 0;
        m_CurrentClock = time;
        m_BeforeClock = time;
        SetClock(time);
        m_StopTime = 0;
    }

    public void Stop()
    {
        m_bRun = false;
        m_StopTime = GetCurrentTime();
    }

    public void Continue()
    {
        if (!m_bRun)
        {
            m_StartTime += GetCurrentTime() - m_StopTime;
            m_bRun = true;
        }
    }

    public void SetAutoStop(bool autostop) { m_bAutoStop = autostop; }

    public void Update(float fDelta)
    {
        float fElapsed; ;

        if (m_bRun)
        {
            m_CurrentTime += fDelta * m_fSpeed;
            fElapsed = (float)(GetCurrentTime() - m_StartTime);
        }
        else
        {
            fElapsed = (float)(m_StopTime - m_StartTime);
        }

        float fTime = m_StartClock + fElapsed;

        if (fTime < 0) fTime = 0;
        if (m_bAutoStop && fTime >= m_StopClock) Stop();

        m_BeforeClock = m_CurrentClock;
        m_CurrentClock = fTime;
    }

    public bool IsOver() { return GetClock() >= m_StopClock; }
    public bool IsRun() { return m_bRun; }
    public bool IsOverRun() { return (IsRun() && IsOver()); }

	public float GetBeforeClock() { return m_BeforeClock; }
	public float GetClock() { return m_CurrentClock; }
	public float GetSpeed() { return m_fSpeed; }

    public void SetClock(float time)
    {
        m_StartClock = time;
        m_StartTime = GetCurrentTime();
    }

    public void SetStopClock(float time) { m_StopClock = time; }
    public void SetSpeed(float speed) { m_fSpeed = speed; }

	protected float GetCurrentTime() { return m_CurrentTime; }
};


public class CCounter
{
    private float m_StartCounter = float.MaxValue;
    private float m_StartTime = 0;
    private float m_StopTime = 0;
    private bool m_bRun = false;
    private bool m_bAutoStop = false;
    private float m_fSpeed = 1.0f;
    private float m_CurrentTime = 0;
    private float m_CurrentCounter = 0;
    private float m_BeforeCounter = 0;

    public CCounter() {}
    public CCounter(bool bStart)
    {
        if (bStart) Start();

    }
    ~CCounter() {}

    public void Start()
    {
        m_bRun = true;
        m_CurrentTime = 0;
        m_CurrentCounter = m_StartCounter;
        m_BeforeCounter = m_StartCounter;
        m_StartTime = GetCurrentTime();
    }

    public void Start(float time)
    {
        m_bRun = true;
        m_CurrentTime = 0;
        m_CurrentCounter = time;
        m_BeforeCounter = time;
        SetCounter(time);
    }

    public void Stop()
    {
        if (m_bRun)
        {
            m_bRun = false;
            m_StopTime = GetCurrentTime();
        }
    }

    public void Continue()
    {
        if (!m_bRun)
        {
            m_StartTime += GetCurrentTime() - m_StopTime;
            m_bRun = true;
        }
    }

    public void SetAutoStop(bool autostop) { m_bAutoStop = autostop; }
    public void Update(float fDelta)
    {
        float fElapsed;

        if (m_bRun)
        {
            m_CurrentTime += fDelta * m_fSpeed;
            fElapsed = (float)(GetCurrentTime() - m_StartTime);
        }
        else
        {
            fElapsed = (float)(m_StopTime - m_StartTime);
        }

        float fCounter = m_StartCounter - fElapsed;

        if (fCounter < 0) fCounter = 0;
        if (m_bAutoStop && fCounter <= 0.0f) Stop();

        m_BeforeCounter = m_CurrentCounter;
        m_CurrentCounter = fCounter;
    }

    public bool IsOver() { return GetCounter() <= 0.0f; }
    public bool IsRun() { return m_bRun; }
	public bool IsOverRun() { return (IsRun() && IsOver()); }

	public float GetBeforeCounter() { return m_BeforeCounter; }
	public float GetCounter() { return m_CurrentCounter; }
	public float GetPercent()
	{
		if (m_StartCounter == 0.0f) return 1.0f;
		float f = 1.0f - GetCounter() / m_StartCounter;
		return f;
	}

	public float GetSpeed() { return m_fSpeed; }
	public void SetCounter(float time)
    {
        m_StartCounter = time;
        m_StartTime = GetCurrentTime();
    }

    public void SetSpeed(float speed) { m_fSpeed = speed; }

    protected float GetCurrentTime() { return m_CurrentTime; }
};

