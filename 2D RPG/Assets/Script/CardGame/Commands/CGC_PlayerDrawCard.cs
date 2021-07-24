using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;

#if CLIENT
using DG.Tweening;
#endif

public class CGC_PlayerDrawCard : CGCommand {
    
    CardData m_cardData;
    int m_cardID;

    public CGC_PlayerDrawCard(CardData cardData, int cardID)
    {
        m_cardData = cardData;
        m_cardID = cardID;
    }

    public CGC_PlayerDrawCard(BKSystem.IO.BitStream packet)
    {
        UnpackCommand(packet);
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)CGCommandID.PLAYER_DRAW_CARD, 0, 16);
        packet.Write(m_cardData);
        packet.Write((ushort)m_cardID, 0, 16);
        Debug.Log("Sending card " + m_cardData.cardName + " with ID " + m_cardID);
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        packet.Read(out m_cardData);
        packet.Read(out m_cardID, 0, 16);
        Debug.Log("Drawing card " + m_cardData.cardName + " with ID " + m_cardID);
    }

    public override void ExecuteAiCommand(AiPlayer aiPlayer, ClientConnectionManager aiConnection)
    {
        aiPlayer.m_cards.Add(new KeyValuePair<int, CardData>(m_cardID, m_cardData));
    }

#if CLIENT

    Sequence m_sequence;
    CardVisual m_card;

    public override void ExecuteCommand()
    {
        // Create the new card
        m_card = m_visualManager.CreateCard(m_cardData, m_cardID, false);

        MoveToDeck();
    }

    void MoveToDeck()
    {
        CGDeckZone deck = m_visualManager.GetPlayerDeck();
        Sequence sequence = deck.MoveCardToZone(m_card, 0.0f);     // Place card on top of deck (instant)
        sequence.Append(m_card.SetFaceDirectionSeq(false, 0.0f));  // Turn card face-down (instant)
        sequence.AppendCallback(MoveToHand);
    }

    void MoveToHand()
    {
        CGHandZone hand = m_visualManager.GetPlayerHand();
        Sequence sequence = hand.MoveCardToZone(m_card);                // Move card to hand
        sequence.Insert(0, m_card.SetFaceDirectionSeq(true, 0.4f));     // Turn card face-up
        sequence.AppendCallback(FinishCommand);
    }

#endif
}
