using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;
using System;

public class SGC_PlayCardFromHand : SGCommand
{

    int m_cardID;
    int m_playerID;

    public SGC_PlayCardFromHand(int cardID)
    {
        m_cardID = cardID;
    }

    public SGC_PlayCardFromHand(BKSystem.IO.BitStream packet, int playerID)
    {
        m_playerID = playerID;
        UnpackCommand(packet);
        Debug.Log("Created SGC_PlayCardFromHand with cardID " + m_cardID + ".");
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)SGCommandID.PLAY_CARD_FROM_HAND, 0, 16);
        packet.Write((ushort)m_cardID, 0, 16);
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        packet.Read(out m_cardID, 0, 16);
    }

    public override void ExecuteCommand(CardGameManager cgm)
    {
        cgm.PlayCardFromHand(m_playerID, m_cardID);
    }
}
