using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;

#if CLIENT
using DG.Tweening;
#endif

/** Set the life total of a player */
public class CGC_SetLife : CGCommand
{
    int m_life;
    int m_playerID;

    public CGC_SetLife(int life, int playerID)
    {
        m_life = life;
        m_playerID = playerID;
    }

    public CGC_SetLife(BKSystem.IO.BitStream packet)
    {
        UnpackCommand(packet);
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)CGCommandID.SET_LIFE, 0, 16);
        packet.Write((ushort)m_life, 0, 16);
        packet.Write((byte)m_playerID);
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        packet.Read(out m_life, 0, 16);
        packet.Read(out m_playerID, 0, 8);
    }

    public override void ExecuteAiCommand(AiPlayer aiPlayer, ClientConnectionManager aiConnection)
    {
        if (m_playerID == aiPlayer.m_playerID)
        {
            aiPlayer.m_lifeTotal = m_life;
        }
        else
        {
            aiPlayer.m_opponentLifeTotal = m_life;
        }
    }

#if CLIENT

    public override void ExecuteCommand()
    {
        if(m_playerID == m_visualManager.m_playerID)
        {
            m_visualManager.GetPlayerLife().SetLife(m_life);
        }
        else
        {
            m_visualManager.GetOpponentLife().SetLife(m_life);
        }
        FinishCommand();
    }

#endif
}
