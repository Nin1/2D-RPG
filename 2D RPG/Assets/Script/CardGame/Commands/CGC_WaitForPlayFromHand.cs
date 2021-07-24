using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;

#if CLIENT
using DG.Tweening;
#endif

public class CGC_WaitForPlayFromHand : CGCommand {

    int m_playerID;
    List<int> m_playableCards;

    public CGC_WaitForPlayFromHand(List<int> playableCardIDs, int playerID) // PlayablecardIDs would be hidden/empty if it's opponent's turn
    {
        m_playerID = playerID;
        m_playableCards = playableCardIDs;
    }

    public CGC_WaitForPlayFromHand(BKSystem.IO.BitStream packet)
    {
        UnpackCommand(packet);
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)CGCommandID.WAIT_FOR_PLAY_FROM_HAND, 0, 16);
        packet.Write((byte)m_playerID);
        packet.Write((ushort)m_playableCards.Count, 0, 16);
        foreach(int card in m_playableCards)
        {
            packet.Write((ushort)card, 0, 16);
        }
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        m_playableCards = new List<int>();
        int numCards;

        packet.Read(out m_playerID, 0, 8);
        packet.Read(out numCards, 0, 16);

        for(int i = 0; i < numCards; i++)
        {
            int cardID;
            packet.Read(out cardID, 0, 16);
            m_playableCards.Add(cardID);
        }
    }

    public override void ExecuteAiCommand(AiPlayer aiPlayer, ClientConnectionManager aiConnection)
    {
        //@TODO: Proper AI card selection
        int cardId = aiPlayer.m_cards[0].Key;
        aiPlayer.m_cards.RemoveAt(0);
        SGC_PlayCardFromHand command = new SGC_PlayCardFromHand(cardId);
        aiConnection.TransmitStream(command.PackCommand());
    }

#if CLIENT

    public override void ExecuteCommand()
    {
        m_visualManager.MakeClickable(m_playableCards, OnCardClick);

        FinishCommand();
    }

    void OnCardClick(CardVisual card)
    {
        // Tell logic server to try to play this card
        SGC_PlayCardFromHand command = new SGC_PlayCardFromHand(card.m_cardID);
        m_visualManager.TransmitStream(command.PackCommand());
        m_visualManager.MakeAllUnclickable();
    }

#endif
}
