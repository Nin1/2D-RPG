using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;

#if CLIENT
using DG.Tweening;
#endif

public class CGC_RequestDeck : CGCommand
{
    public CGC_RequestDeck()
    {
    }

    public CGC_RequestDeck(BKSystem.IO.BitStream packet)
    {
        UnpackCommand(packet);
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)CGCommandID.REQUEST_DECK, 0, 16);
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        // Nothing to unpack (in the future this could include deck restrictions)
    }

    public override void ExecuteAiCommand(AiPlayer aiPlayer, ClientConnectionManager aiConnection)
    {
        // @TODO: AI implementation
        // Sending the same deck that the player loaded for now
        string deckFileName = m_visualManager.GetDeckFileName();
        PackedDeck deck = new PackedDeck();
        deck.LoadFromJSON(PackedDeck.deckPath + deckFileName);
        SGC_SendDeck command = new SGC_SendDeck(deck);
        m_visualManager.TransmitStream(command.PackCommand());
    }

#if CLIENT

    public override void ExecuteCommand()
    {
        // @TODO: Open deck picker interface

        // For now, send the deck that was picked in the connect menu
        string deckFileName = m_visualManager.GetDeckFileName();
        PackedDeck deck = new PackedDeck();
        deck.LoadFromJSON(PackedDeck.deckPath + deckFileName);
        SGC_SendDeck command = new SGC_SendDeck(deck);
        m_visualManager.TransmitStream(command.PackCommand());

        FinishCommand();
    }

#endif
}
