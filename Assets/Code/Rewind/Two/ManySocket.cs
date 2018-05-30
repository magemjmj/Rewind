using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

using System;
using BestHTTP;
using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;

using StateMachine;
using FlatBuffers;

public class ManySocket : Singleton<ManySocket>
{
    public string m_ConnectUrl;

    public PlayerSync[] m_players;
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
        uint playerId = Convert.ToUInt32(args[0]);
        byte[] arr = packet.Attachments[0];

        //Inputs inputs = Inputs.FromBytes(arr);
        ByteBuffer bb = new ByteBuffer(arr);
        FInputs finputs = FInputs.GetRootAsFInputs(bb);

        Inputs inputs = new Inputs();
        inputs.frame = finputs.Frame;
        inputs.up = finputs.Up;
        inputs.down = finputs.Down;
        inputs.left = finputs.Left;
        inputs.right = finputs.Right;
        inputs.jump = finputs.Jump;

        m_players[playerId].ReceiveInputFromServer(inputs);
    }

    public void SendInputs(string eventname, uint playerid, Inputs input)
    {
        //byte[] arr = input.GetBytes();
        FlatBufferBuilder fbb = new FlatBufferBuilder(1);
        FInputs.StartFInputs(fbb);
        FInputs.AddFrame(fbb, input.frame);
        FInputs.AddUp(fbb, input.up);
        FInputs.AddDown(fbb, input.down);
        FInputs.AddLeft(fbb, input.left);
        FInputs.AddRight(fbb, input.right);
        FInputs.AddJump(fbb, input.jump);
        FInputs.FinishFInputsBuffer(fbb, FInputs.EndFInputs(fbb));

        SocketIOManager.GetManager().Emit("sendinput", playerid, fbb.SizedByteArray());
    }

    void GameState_Enter()
    {
        foreach (PlayerSync player in m_players)
        {
            player.gameObject.SetActive(true);
        }

        m_physics.SetActive(true);
    }

}
