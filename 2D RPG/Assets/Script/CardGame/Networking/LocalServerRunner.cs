using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using BKSystem.IO;
using UnityEngine;

public class LocalServerRunner {

    CardGameServer m_server;
    ClientConnectionManager m_clientConMgr;
    ClientConnectionManager m_aiConMgr; // AI connection
    AiPlayer m_aiPlayer;

    public void StartServer(ClientConnectionManager connectionMgr)
    {
        m_clientConMgr = connectionMgr;
        m_aiConMgr = new ClientConnectionManager();
        m_aiPlayer = new AiPlayer();
        m_server = new CardGameServer();

        InitializeServer();
        ConnectClients();
        RunServer();
    }

    private void InitializeServer()
    {
        IPAddress localhost = Dns.GetHostEntry("127.0.0.1").AddressList[0];
        int port = 8080;    // unused
        m_server.InitialiseConnection(localhost, port);
    }

    private void ConnectClients()
    {
        m_server.ConnectLocal(m_clientConMgr, 0);
        m_server.ConnectLocal(m_aiConMgr, 1);
    }

    private void RunServer()
    {
        Thread server = new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            m_server.RunServer();
        });
        server.Start();
    }

    public void AiUpdate()
    {
        // See ClientConnectionInterface.Update()
        if (m_aiConMgr != null && m_aiConMgr.IsConnected())
        {
            CGCommand command = m_aiConMgr.ReceiveCommand();
            if (command != null)
            {
                command.OnReceivedAi(m_aiPlayer, m_aiConMgr);
                command.ExecuteAiCommand(m_aiPlayer, m_aiConMgr);
            }
        }
    }
}
