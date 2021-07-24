using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;

#if CLIENT
using DG.Tweening;
#endif

public struct SpellChannelData
{
    public int cardID;
    public int playerID;
    public int newChannel;
}

public class CGC_ChannelSpell : CGCommand {

    List<SpellChannelData> m_data = new List<SpellChannelData>();

    public CGC_ChannelSpell(List<SpellChannelData> spells)
    {
        m_data = spells;
    }

    public CGC_ChannelSpell(BKSystem.IO.BitStream packet)
    {
        UnpackCommand(packet);
    }

    public CGC_ChannelSpell(int cardID, int controllerID, int newChannel)
    {
        SpellChannelData data;
        data.cardID = cardID;
        data.playerID = controllerID;
        data.newChannel = newChannel;

        m_data = new List<SpellChannelData>();
        m_data.Add(data);
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)CGCommandID.CHANNEL_SPELL, 0, 16);
        packet.Write(m_data.Count, 0, 16);
        foreach(SpellChannelData data in m_data)
        {
            packet.Write((ushort)data.cardID, 0, 16);
            packet.Write((byte)data.playerID);
            packet.Write((ushort)data.newChannel, 0, 5);
        }
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        int dataCount;
        packet.Read(out dataCount, 0, 16);
        for(int i = 0; i < dataCount; i++)
        {
            SpellChannelData data = new SpellChannelData();
            packet.Read(out data.cardID, 0, 16);
            packet.Read(out data.playerID, 0, 8);
            packet.Read(out data.newChannel, 0, 5);
            m_data.Add(data);

        }
    }

    public override void ExecuteAiCommand(AiPlayer aiPlayer, ClientConnectionManager aiConnection)
    {
        // @TODO: AI implementation
    }

#if CLIENT

    public override void ExecuteCommand()
    {
        if(m_data.Count == 0)
        {
            FinishCommand();
            return;
        }

        Sequence seq = DOTween.Sequence();

        foreach (SpellChannelData spell in m_data)
        {
            CardVisual card = m_visualManager.GetCard(spell.cardID);
            CGZone channel;

            if (spell.playerID == m_visualManager.m_playerID)
            {
                channel = m_visualManager.GetPlayerChannel(spell.newChannel);
            }
            else
            {
                channel = m_visualManager.GetOpponentChannel(spell.newChannel);
            }

            channel.MoveCardToZone(card).Kill();
        }

        foreach (CGChannelZone channel in m_visualManager.m_playerChannels)
        {
            seq.Insert(0, channel.AdjustCardPositionsSeq(CGZone.DEFAULT_DURATION));
        }
        foreach (CGChannelZone channel in m_visualManager.m_opponentChannels)
        {
            seq.Insert(0, channel.AdjustCardPositionsSeq(CGZone.DEFAULT_DURATION));
        }

        seq.AppendCallback(FinishCommand);
    }

#endif
}
