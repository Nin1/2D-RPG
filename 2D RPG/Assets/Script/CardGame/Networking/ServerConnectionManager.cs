using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using BKSystem.IO;

/** 
 * Super basic Connection Manager
 * Can connect to 2 clients, transmit data to each, and receives data from each */
public class ServerConnectionManager
{
    /*
    private static ServerConnectionManager instance;

    public static ServerConnectionManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new ServerConnectionManager();
            }
            return instance;
        }
    }*/

    IPAddress m_ip;
    public int m_port { get; private set; }

    TcpClient[] m_clients = new TcpClient[2];

    TcpListener server;

    public void InitialiseServer(IPAddress ipAddress, int port)
    {
        m_ip = ipAddress;
        m_port = port;

        m_clients[0] = default(TcpClient);
        m_clients[1] = default(TcpClient);

        server = new TcpListener(m_ip, m_port);

        try
        {
            server.Start();
            Console.WriteLine("Server started");
            Console.WriteLine("Listening on port " + port);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

    public void Close()
    {
        server.Stop();
        foreach (TcpClient c in m_clients)
        {
            if (c != null)
            {
                c.Close();
            }
        }
    }

    public void ConnectToClient(int index)
    {
        Debug.Log("Connecting to client " + index);

        TcpClient client = default(TcpClient);

        while (client == null)
        {
            client = server.AcceptTcpClient();
        }

        Debug.Log("Connected to client " + index);

        m_clients[index] = client;
    }

    public void TransmitStream(BKSystem.IO.BitStream data, int player)
    {
        if (m_clients[player] != null && m_clients[player].Connected)
        {
            // Pad the data by 1 bit ???
            data.Write(0);

            // Send how large the data is in the first 32 bits
            byte[] sendData = data.ToByteArray();
            int sendLength = IPAddress.HostToNetworkOrder(sendData.Length);
            byte[] lengthData = BitConverter.GetBytes(sendLength);

            // Choose the stream to use
            NetworkStream stream;

            stream = m_clients[player].GetStream();

            stream.Flush();

            stream.Write(lengthData, 0, lengthData.Length);
            //Debug.Log("Sending " + sendData.Length + " bytes to player " + player);

            // Then send the data
            stream.Write(sendData, 0, sendData.Length);
        }
        else
        {
            Debug.LogWarning("Attempting to transmit data while disconnected");
        }
    }

    public SGCommand ReceiveStream(int player)
    {
        NetworkStream stream;

        if (m_clients[player] != null && m_clients[player].Connected)
        {
            stream = m_clients[player].GetStream();
        }
        else
        {
            Debug.LogWarning("Trying to receive stream from disconnected player " + player);
            return null;
        }

        if (stream.DataAvailable)
        {
            // Decode the first 32 bits to find the length of the remaining data
            byte[] dataSizeBytes = new byte[4];
            stream.Read(dataSizeBytes, 0, 4);
            int dataSize = BitConverter.ToInt32(dataSizeBytes, 0);
            dataSize = IPAddress.NetworkToHostOrder(dataSize);
            //Debug.Log("Received data of size " + dataSize + " bytes");

            // Receive the data
            // Convert size to bytes
            byte[] data = new byte[dataSize];
            stream.Read(data, 0, dataSize);

            // Put the data into a BitStream
            BKSystem.IO.BitStream clientStream = new BKSystem.IO.BitStream(dataSize * 8);
            clientStream.Write(data);

            if (clientStream.Length > 0)
            {
                return SGCommand.CreateCommandFromPacket(clientStream, player);
            }
        }
        return null;
    }

    // TODO: This doesn't work right
    public bool Connected()
    {
        return m_clients[0].Connected && m_clients[1].Connected;
    }
}
