using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;

// Request a message from the client to refresh their timeout

public class CGC_RefreshTimeout : CGCommand
{
    public CGC_RefreshTimeout(int playerID)
    {
    }

    public CGC_RefreshTimeout(BKSystem.IO.BitStream packet)
    {
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)CGCommandID.REFRESH_TIMEOUT, 0, 16);
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
    }

    public override void ExecuteAiCommand(AiPlayer aiPlayer, ClientConnectionManager aiConnection)
    {
    }

    public override void OnReceivedAi(AiPlayer aiPlayer, ClientConnectionManager aiConnection)
    {
        SGC_RefreshTimeout command = new SGC_RefreshTimeout();
        aiConnection.TransmitStream(command.PackCommand());
    }

#if CLIENT

    public override void OnReceived()
    {
        SGC_RefreshTimeout command = new SGC_RefreshTimeout();
        m_visualManager.TransmitStream(command.PackCommand());
    }

    public override void ExecuteCommand()
    {
        FinishCommand();
    }

#endif
}
