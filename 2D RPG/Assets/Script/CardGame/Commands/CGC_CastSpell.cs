using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;
using System;

#if CLIENT
using DG.Tweening;
#endif

public class CGC_CastSpell : CGCommand
{
    int m_cardID;
    int m_casterID;

    public CGC_CastSpell(int cardID, int casterID)
    {
        m_cardID = cardID;
        m_casterID = casterID;
    }

    public CGC_CastSpell(BKSystem.IO.BitStream packet)
    {
        UnpackCommand(packet);
        Debug.Log("Created CGC_CastSpell with cardID " + m_cardID + " and playerID " + m_casterID + ".");
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)CGCommandID.CAST_SPELL, 0, 16);
        packet.Write((ushort)m_cardID, 0, 16);
        packet.Write((byte)m_casterID);
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        packet.Read(out m_cardID, 0, 16);   // The first 16 bytes contain the card ID
        packet.Read(out m_casterID, 0, 8);  // The next 8 bytes contain the caster ID
    }

    public override void ExecuteAiCommand(AiPlayer aiPlayer, ClientConnectionManager aiConnection)
    {
        // @TODO: AI implementation
    }


#if CLIENT
    public override void ExecuteCommand()
    {
        CGBasicZone playerFocusZone;
        CardVisual card = m_visualManager.GetCard(m_cardID);

        if (card != null)
        {
            if (m_casterID == m_visualManager.m_playerID)
            {
                playerFocusZone = m_visualManager.GetPlayerTargettingZone();
            }
            else
            {
                playerFocusZone = m_visualManager.GetOpponentTargettingZone();
            }

            Sequence seq = playerFocusZone.MoveCardToZone(card);
            seq.AppendInterval(1.0f);
            seq.AppendCallback(RunSpellAnimation);
        }
        else
        {
            Debug.LogError("CardVisual being cast not found");
            FinishCommand();
        }
    }

    protected virtual void RunSpellAnimation()
    {
        FinishCommand();
    }
#endif
}
