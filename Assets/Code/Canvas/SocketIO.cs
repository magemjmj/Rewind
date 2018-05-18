using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

using System;
using BestHTTP;
using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;

using StateMachine;

public class SocketIO : MonoBehaviour
{
    public string m_ConnectUrl;

    public PlayerSync m_player;
    public remote m_remote;

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
    void Start () {
        SocketIOManager.GetManager().Create(m_ConnectUrl + "/socket.io/");
        SocketIOManager.GetManager().On(SocketIOEventTypes.Connect, OnConnect);
        SocketIOManager.GetManager().On(SocketIOEventTypes.Disconnect, OnDisconnect);

        SocketIOManager.GetManager().On("login", OnLogin);
        SocketIOManager.GetManager().On("receiveinput", OnReceive);

        m_fsm.ChangeState(ScenarioStates.OffState);
    }
	
	// Update is called once per frame
	void Update () {
		
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

        Dictionary<string, object> loginData = new Dictionary<string, object>();
        loginData.Add("userid", m_szUserId);
        SocketIOManager.GetManager().Emit("login", loginData);
    }

    void OnDisconnect(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("OnDisconnect");
    }

    void OnLogin(Socket socket, Packet packet, params object[] args)
    {
        Dictionary<string, object> data = args[0] as Dictionary<string, object>;

        Debug.Log("OnLogin : ");

        m_fsm.ChangeState(ScenarioStates.GameState);
    }


    void OnReceive(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("OnReceive : ");
        byte[] arr = packet.Attachments[0];
        Inputs inputs = Inputs.FromBytes(arr);

        m_remote.ReceiveInputFromServer(inputs);
    }

    void GameState_Enter()
    {
        m_player.gameObject.SetActive(true);
        m_remote.gameObject.SetActive(true);
    }

}
