using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;

#if CLIENT
using DG.Tweening;
#endif

public class CGC_OpponentDrawCard : CGCommand
{

    public CGC_OpponentDrawCard()
    {
    }

    public CGC_OpponentDrawCard(BKSystem.IO.BitStream packet)
    {
        UnpackCommand(packet);
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)CGCommandID.OPPONENT_DRAW_CARD, 0, 16);
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        // Nothing to unpack
    }

    public override void ExecuteAiCommand(AiPlayer aiPlayer, ClientConnectionManager aiConnection)
    {
        aiPlayer.m_opponentCardsInHand += 1;
    }

#if CLIENT

    Sequence m_sequence;
    CardVisual m_card;

    public override void ExecuteCommand()
    {
        // Create the new card
        m_card = m_visualManager.CreateHiddenCard();

        MoveToDeck();
    }

    void MoveToDeck()
    {
        CGDeckZone deck = m_visualManager.GetOpponentDeck();
        Sequence sequence = deck.MoveCardToZone(m_card, 0.0f);     // Place card on top of deck (instant)
        sequence.AppendCallback(MoveToHand);
    }

    void MoveToHand()
    {
        CGHandZone hand = m_visualManager.GetOpponentHand();
        Sequence sequence = hand.MoveCardToZone(m_card);                // Move card to hand
        sequence.AppendCallback(FinishCommand);
    }

#endif
}