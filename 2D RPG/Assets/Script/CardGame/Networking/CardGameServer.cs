using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

public class CardGameServer
{
    const int CONNECTION_TIMEOUT_MS = 2000;
    const int MAIN_LOOP_SLEEP_MS = 100;
    const int CONNECTION_TIMEOUT_LOOPS = CONNECTION_TIMEOUT_MS / MAIN_LOOP_SLEEP_MS;

    CardGameManager m_cgManager;
    ServerConnectionManager m_connection;

    int[] m_playerTimeouts = new int[2];

    public CardGameServer()
    {
        m_connection = new ServerConnectionManager();
        m_playerTimeouts[0] = CONNECTION_TIMEOUT_LOOPS;
        m_playerTimeouts[1] = CONNECTION_TIMEOUT_LOOPS;
    }

    public void InitialiseConnection(IPAddress ip, int port)
    {
        m_connection.InitialiseServer(ip, port);
    }

    public bool ConnectLocal(ClientConnectionManager client, int id)
    {
        Debug.Log("Connecting local player " + id);

        object locker = new object();

        // Make server listen for clients on separate thread
        Thread connectToClient = new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            lock (locker)
            {
                m_connection.ConnectToClient(id);
            }
        });
        connectToClient.Start();

        // Make client attempt to connect
        int attempts = 100;
        while (true)
        {
            if (client.AttemptConnection("127.0.0.1", m_connection.m_port))
            {
                break;
            }
            if (attempts < 0)
            {
                Debug.Log("Failed to connect local player " + id);
                return false;
            }
            attempts--;
            // Wait a bit before trying again
            Thread.Sleep(10);
        }

        lock (locker)
        {
            return true;
        }
    }

    public void RunServer()
    {
        TransmitPlayerIDs();
        InitialiseGame();

        bool playing = true;

        while (playing)
        {
            Thread.Sleep(MAIN_LOOP_SLEEP_MS);

            HandlePlayer(0);
            HandlePlayer(1);

            // If either player has timed out, close the server
            foreach (int timeout in m_playerTimeouts)
            {
                if (timeout <= 0)
                {
                    Debug.Log("Player timed out.");
                    playing = false;
                }
            }
        }

        Debug.Log("Closing server");
        m_connection.Close();
    }

    void TransmitPlayerIDs()
    {
        CGC_SetPlayerID setPlayer0 = new CGC_SetPlayerID(0);
        m_connection.TransmitStream(setPlayer0.PackCommand(), 0);
        CGC_SetPlayerID setPlayer1 = new CGC_SetPlayerID(1);
        m_connection.TransmitStream(setPlayer1.PackCommand(), 1);
    }

    void InitialiseGame()
    {
        m_cgManager = new CardGameManager(m_connection);
        m_cgManager.RunGameLogic();
    }

    void HandlePlayer(int id)
    {
        SGCommand playerCommand = m_connection.ReceiveStream(id);
        if (playerCommand != null)
        {
            // Execute the command
            if (playerCommand.GetID() == SGCommandID.REFRESH_TIMEOUT)
            {
                m_playerTimeouts[id] = CONNECTION_TIMEOUT_LOOPS;
            }
            else
            {
                playerCommand.ExecuteCommand(m_cgManager);
                m_playerTimeouts[id]--;
            }
        }
        else
        {
            // Ask player for a no-timeout packet
            CGC_RefreshTimeout timeout = new CGC_RefreshTimeout(id);
            m_connection.TransmitStream(timeout.PackCommand(), id);
            m_playerTimeouts[id]--;
        }
    }
}
