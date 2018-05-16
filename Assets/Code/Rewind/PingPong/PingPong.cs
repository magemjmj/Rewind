using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

using System;
using BestHTTP;
using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;

using StateMachine;

public class PingPong : MonoBehaviour
{
    public string m_ConnectUrl;

    public enum ScenarioStates
    {
        OffState,
        PingState,
        PongState,
    }

    private StateMachine<ScenarioStates> m_fsm;

    string m_szUserId;

    void Awake()
    {
        m_fsm = StateMachine<ScenarioStates>.Initialize(this);
    }

    // Use this for initialization
    void Start()
    {
        SocketIOManager.GetManager().Create(m_ConnectUrl + "/socket.io/");
        SocketIOManager.GetManager().On(SocketIOEventTypes.Connect, OnConnect);
        SocketIOManager.GetManager().On(SocketIOEventTypes.Disconnect, OnDisconnect);

        SocketIOManager.GetManager().On("pongs", OnPong);

        m_fsm.ChangeState(ScenarioStates.OffState);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Off
    /// </summary>
    void OffState_Enter()
    {
        SocketIOManager.GetManager().Connect();
    }

    void OnConnect(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("OnConnect");

        m_fsm.ChangeState(ScenarioStates.PingState);
    }

    void OnDisconnect(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("OnDisconnect");

        m_fsm.ChangeState(ScenarioStates.OffState);
    }


    void OnPong(Socket socket, Packet packet, params object[] args)
    {
        m_fsm.ChangeState(ScenarioStates.PingState);
    }

    void PingState_Enter()
    {
        Debug.Log("Ping");
        SocketIOManager.GetManager().Emit("pings");
        m_fsm.ChangeState(ScenarioStates.PongState);
    }

    void PongState_Enter()
    {
        Debug.Log("Pong");
    }

}
