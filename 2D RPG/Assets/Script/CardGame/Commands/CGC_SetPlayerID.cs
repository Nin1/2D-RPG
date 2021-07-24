using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;

#if CLIENT
using DG.Tweening;
#endif

public class CGC_SetPlayerID : CGCommand
{
    int m_playerID;

    public CGC_SetPlayerID(int playerID)
    {
        m_playerID = playerID;
    }

    public CGC_SetPlayerID(BKSystem.IO.BitStream packet)
    {
        UnpackCommand(packet);
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)CGCommandID.SET_PLAYER_ID, 0, 16);
        packet.Write((byte)m_playerID);
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        packet.Read(out m_playerID, 0, 8);
    }

    public override void ExecuteAiCommand(AiPlayer aiPlayer, ClientConnectionManager aiConnection)
    {
        aiPlayer.m_playerID = m_playerID;
    }

#if CLIENT

    public override void ExecuteCommand()
    {
        m_visualManager.SetPlayerID(m_playerID);
        FinishCommand();
    }

#endif
}
