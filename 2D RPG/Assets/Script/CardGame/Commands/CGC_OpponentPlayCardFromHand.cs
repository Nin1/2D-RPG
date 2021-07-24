using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;

#if CLIENT
using DG.Tweening;
#endif

public class CGC_OpponentPlayCardFromHand : CGCommand
{
    CardData m_cardData;
    int m_cardID;
    int m_channel;

    public CGC_OpponentPlayCardFromHand(CardData cardData, int cardID, int channel)
    {
        m_cardData = cardData;
        m_cardID = cardID;
        m_channel = channel;
    }

    public CGC_OpponentPlayCardFromHand(BKSystem.IO.BitStream packet)
    {
        UnpackCommand(packet);
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)CGCommandID.OPPONENT_PLAY_CARD_FROM_HAND, 0, 16);
        packet.Write(m_cardData);
        packet.Write((ushort)m_cardID, 0, 16);
        packet.Write((ushort)m_channel, 0, 5);
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        packet.Read(out m_cardData);
        packet.Read(out m_cardID, 0, 16);
        packet.Read(out m_channel, 0, 5);
    }

    public override void ExecuteAiCommand(AiPlayer aiPlayer, ClientConnectionManager aiConnection)
    {
        // @TODO: AI implementation
    }

#if CLIENT

    CardVisual m_card;

    public override void ExecuteCommand()
    {
        CGZone hand = m_visualManager.GetOpponentHand();
        m_card = hand.GetFirstCard();

        if (m_card == null)
        {
            Debug.LogError("Card played does not exist in hand");

            // Create a new card
            m_card = m_visualManager.CreateCard(m_cardData, m_cardID, false);
            MoveToHand();
        }

        m_visualManager.DefineCard(m_card, m_cardData, m_cardID);
        MoveToChannel();
    }

    void MoveToHand()
    {
        CGHandZone hand = m_visualManager.GetPlayerHand();
        Sequence sequence = hand.MoveCardToZone(m_card, 0.0f);  // Move card to hand
        sequence.AppendCallback(MoveToChannel);
    }

    void MoveToChannel()
    {
        CGZone channel = m_visualManager.GetOpponentChannel(m_channel);
        Sequence sequence = channel.MoveCardToZone(m_card);         // Move card to channel
        sequence.Insert(0, m_card.SetFaceDirectionSeq(true, 0.4f)); // Turn card face-up
        sequence.AppendCallback(FinishCommand);
    }

#endif
}
