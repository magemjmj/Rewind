using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

using System;
using BestHTTP;
using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;

using StateMachine;

public class CloneSocket : MonoBehaviour
{
    public string m_ConnectUrl;

    public Clone m_player;
    public GameObject m_physics;

    public enum ScenarioStates
    {
        OffState,
        LoginState,
        GameState,
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

        SocketIOManager.GetManager().On("login", OnLogin);
        SocketIOManager.GetManager().On("receiveinput", OnReceive);

        m_fsm.ChangeState(ScenarioStates.OffState);

        Physics.autoSimulation = false;
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

        SocketIOManager.GetManager().Emit("login");
    }

    void OnDisconnect(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("OnDisconnect");
    }

    void OnLogin(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("OnLogin : ");
        m_fsm.ChangeState(ScenarioStates.GameState);
    }


    void OnReceive(Socket socket, Packet packet, params object[] args)
    {
        byte[] arr = packet.Attachments[0];
        Inputs inputs = Inputs.FromBytes(arr);

        m_player.ReceiveInputFromServer(inputs);
    }

    void GameState_Enter()
    {
        m_player.gameObject.SetActive(true);
        m_physics.SetActive(true);
    }

}
