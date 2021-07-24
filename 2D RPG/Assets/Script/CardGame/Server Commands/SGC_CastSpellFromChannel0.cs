using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;
using System;

public class SGC_CastSpellFromChannel0 : SGCommand {

    int m_cardID;
    int m_targetID;
    int m_playerID;

    public SGC_CastSpellFromChannel0(int cardID, int targetID)
    {
        m_cardID = cardID;
        m_targetID = targetID;
    }

    public SGC_CastSpellFromChannel0(BKSystem.IO.BitStream packet, int playerID)
    {
        m_playerID = playerID;
        UnpackCommand(packet);
        Debug.Log("Created SGC_CastSpellFromChannel0 with cardID " + m_cardID + ".");
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)SGCommandID.CAST_SPELL_FROM_0, 0, 16);
        packet.Write((ushort)m_cardID, 0, 16);
        packet.Write(m_targetID >= 0);
        if (m_targetID >= 0)
        {
            packet.Write((ushort)m_targetID, 0, 16);
        }
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        packet.Read(out m_cardID, 0, 16);

        bool hasTarget;
        packet.Read(out hasTarget);

        if (hasTarget)
        {
            packet.Read(out m_targetID, 0, 16);
        }
        else
        {
            m_targetID = -1;
        }
    }

    public override void ExecuteCommand(CardGameManager cgm)
    {
        cgm.CastSpellFromChannel0(m_cardID, m_targetID, m_playerID);
    }
}
