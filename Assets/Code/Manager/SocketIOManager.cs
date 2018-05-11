using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP;
using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;
using BestHTTP.SocketIO.JsonEncoders;


public class SocketIOManager : Singleton<SocketIOManager>
{
    SocketManager m_SocketManager = null;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        if (m_SocketManager != null)
        {
            m_SocketManager.Close();
            m_SocketManager = null;
        }
    }

    public void Create(string strUrl)
    {
        SocketOptions options = new SocketOptions();
        options.AutoConnect = false;
		options.ConnectWith = BestHTTP.SocketIO.Transports.TransportTypes.WebSocket;

        m_SocketManager = new SocketManager(new Uri(strUrl), options);
		//m_SocketManager.Encoder = new JsonDotNetEncoder ();
		m_SocketManager.Encoder = new BestHTTP.SocketIO.JsonEncoders.LitJsonEncoder();
        //Socket sockChat = manager.GetSocket("/socket.io"); 
        //manager.Socket.On(SocketIOEventTypes.Error, (socket, packet, args) => Debug.LogError(string.Format("Error: {0}", args[0].ToString())));
        m_SocketManager.Socket.On(SocketIOEventTypes.Connect, OnServerConnect);
        m_SocketManager.Socket.On(SocketIOEventTypes.Disconnect, OnServerDisconnect);
        m_SocketManager.Socket.On(SocketIOEventTypes.Error, OnError);
        m_SocketManager.Socket.On("reconnect", OnReconnect);
        m_SocketManager.Socket.On("reconnecting", OnReconnecting);
        m_SocketManager.Socket.On("reconnect_attempt", OnReconnectAttempt);
        m_SocketManager.Socket.On("reconnect_failed", OnReconnectFailed);

        m_SocketManager.Socket.On("message_string", OnMessageString);
    }

    public void Connect()
    {
        m_SocketManager.Open();
    }

    public void Connect(string strUrl)
    {
        Create(strUrl);
        Connect();
    }

	public Socket GetSocket(string strNameSpace)
	{
		return m_SocketManager.GetSocket (strNameSpace);
	}

    public void On(string eventName, SocketIOCallback fn)
    {
        m_SocketManager.Socket.On(eventName, fn);
    }

    public void On(SocketIOEventTypes eventTypes, SocketIOCallback fn)
    {
        m_SocketManager.Socket.On(eventTypes, fn);
    }

    public void Off(string eventName, SocketIOCallback fn)
    {
        m_SocketManager.Socket.Off(eventName, fn);
    }

    public void Off(SocketIOEventTypes eventTypes, SocketIOCallback fn)
    {
        m_SocketManager.Socket.Off(eventTypes, fn);
    }

    public void Emit(string eventName, params object[] args)
    {
        m_SocketManager.Socket.Emit(eventName, args);
    }

    void OnMessageString(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("OnMessage : " + args[0].ToString());
    }

    void OnServerConnect(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Connected");
    }

    void OnServerDisconnect(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Disconnected");
    }

    void OnError(Socket socket, Packet packet, params object[] args)
    {
        Error error = args[0] as Error;

        switch (error.Code)
        {
            case SocketIOErrors.User:
                Debug.LogWarning("Exception in an event handler!");
                break;
            case SocketIOErrors.Internal:
                Debug.LogWarning("Internal error!");
                break;
            default:
                Debug.LogWarning("server error!");
                break;
        }
    }

    void OnReconnect(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Reconnected");
    }

    void OnReconnecting(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Reconnecting");
    }

    void OnReconnectAttempt(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("ReconnectAttempt");
    }

    void OnReconnectFailed(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("ReconnectFailed");
    }

    public void SendMessageString(string strMessage)
    {
        m_SocketManager.Socket.Emit("message_string", strMessage);
    }
}
