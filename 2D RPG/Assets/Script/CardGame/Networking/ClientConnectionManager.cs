using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using BKSystem.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ClientConnectionManager {
    
    public bool m_offline = false;

    TcpClient m_client;

    ~ClientConnectionManager()
    {
        m_client.Close();
    }

    public bool IsConnected()
    {
        return m_client.Connected;
    }
    
    public bool AttemptConnection(string ipAddress, int port)
    {
        m_client = new TcpClient(ipAddress, port);
        if(m_client.Connected)
        {
            Debug.Log("Connected to server!");
            return true;
        }
        return false;
    }

    public CGCommand ReceiveCommand()
    {
        NetworkStream stream = m_client.GetStream();
        while (stream.DataAvailable)
        {
            //Debug.Log("Data available! Reading stream...");
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
            BKSystem.IO.BitStream serverStream = new BKSystem.IO.BitStream(dataSize * 8);
            serverStream.Write(data);

            if (serverStream.Length > 0)
            {
                return CGCommand.CreateCommandFromPacket(serverStream);
            }
        }
        return null;
    }

    public void TransmitStream(BKSystem.IO.BitStream data)
    {
        if(m_client.Connected)
        {
            // Pad the data by 1 bit ???
            data.Write(0);

            // Send how large the data is in the first 32 bits
            byte[] sendData = data.ToByteArray();
            int sendLength = IPAddress.HostToNetworkOrder(sendData.Length);
            byte[] lengthData = BitConverter.GetBytes(sendLength);
            m_client.GetStream().Flush();

            m_client.GetStream().Write(lengthData, 0, lengthData.Length);
            Debug.Log("Sending " + sendData.Length + " bytes to server");

            // Then send the data
            m_client.GetStream().Write(sendData, 0, sendData.Length);
        }
    }
}
