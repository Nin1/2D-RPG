using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;

#if CLIENT
using DG.Tweening;
#endif

public class CGC_MoveCardToGraveyard : CGCommand {

    int m_cardID;
    int m_playerID;

    public CGC_MoveCardToGraveyard(int graveyardPlayerID, int cardID)
    {
        m_cardID = cardID;
        m_playerID = graveyardPlayerID;
    }

    public CGC_MoveCardToGraveyard(BKSystem.IO.BitStream packet)
    {
        UnpackCommand(packet);
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)CGCommandID.MOVE_CARD_TO_GRAVEYARD, 0, 16);
        packet.Write((byte)m_playerID);
        packet.Write((ushort)m_cardID, 0, 16);
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        packet.Read(out m_playerID, 0, 8);
        packet.Read(out m_cardID, 0, 16);
    }

    public override void ExecuteAiCommand(AiPlayer aiPlayer, ClientConnectionManager aiConnection)
    {
        // @TODO: AI implementation
    }

#if CLIENT

    public override void ExecuteCommand()
    {
        CGBasicZone graveyard;
        CardVisual card = m_visualManager.GetCard(m_cardID);
        if (card != null)
        {

            if (m_playerID == m_visualManager.m_playerID)
            {
                graveyard = m_visualManager.GetPlayerGraveyard();
            }
            else
            {
                graveyard = m_visualManager.GetOpponentGraveyard();
            }

            MoveCardToGraveyardWithCallback(graveyard, card);
        }
        else
        {
            Debug.LogError("Card to move to graveyard not found");
            FinishCommand();
        }
    }

    void MoveCardToGraveyardWithCallback(CGBasicZone graveyard, CardVisual card)
    {
        DOTween.Sequence()
            .Append(graveyard.MoveCardToZone(card))
            .AppendCallback(FinishCommand);
    }

#endif
}
