using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;
using System;

public class SGC_SendDeck : SGCommand
{
    PackedDeck m_deck;
    int m_playerID;

    public SGC_SendDeck(PackedDeck deck)
    {
        m_deck = deck;
    }

    public SGC_SendDeck(BKSystem.IO.BitStream packet, int playerID)
    {
        m_playerID = playerID;
        UnpackCommand(packet);
        Debug.Log("Created SGC_SendDeck");
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)SGCommandID.SEND_DECK, 0, 16);
        packet.Write(m_deck);
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        packet.Read(out m_deck);
    }

    public override void ExecuteCommand(CardGameManager cgm)
    {
        cgm.AssignDeck(m_deck, m_playerID);
    }
}
