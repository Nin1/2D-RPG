using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BKSystem.IO;

#if CLIENT
using DG.Tweening;
#endif

public struct CastableCard
{
    public int cardID;
    public List<int> targets;
}

public class CGC_WaitForCastSelection : CGCommand {

    List<CastableCard> m_castableCards = new List<CastableCard>();

    public CGC_WaitForCastSelection(List<CastableCard> castableCards) // PlayablecardIDs would be hidden/empty if it's opponent's turn
    {
        m_castableCards = castableCards;
    }

    public CGC_WaitForCastSelection(BKSystem.IO.BitStream packet)
    {
        UnpackCommand(packet);
    }

    public override BKSystem.IO.BitStream PackCommand()
    {
        BKSystem.IO.BitStream packet = new BKSystem.IO.BitStream();
        packet.Write((ushort)CGCommandID.WAIT_FOR_CAST_SELECTION, 0, 16);
        packet.Write((ushort)m_castableCards.Count, 0, 16);
        foreach(CastableCard card in m_castableCards)
        {
            packet.Write(card);
        }
        return packet;
    }

    public override void UnpackCommand(BKSystem.IO.BitStream packet)
    {
        m_castableCards = new List<CastableCard>();

        int numCards;
        packet.Read(out numCards, 0, 16);
        for(int i = 0; i < numCards; i++)
        {
            CastableCard card;
            packet.Read(out card);
            m_castableCards.Add(card);
        }
    }

    public override void ExecuteAiCommand(AiPlayer aiPlayer, ClientConnectionManager aiConnection)
    {
        int cardID = m_castableCards[0].cardID;
        int targetID = -1;
        if(m_castableCards[0].targets.Count > 0)
        {
            targetID = m_castableCards[0].targets[0];
        }
        SGC_CastSpellFromChannel0 command = new SGC_CastSpellFromChannel0(cardID, targetID);
        aiConnection.TransmitStream(command.PackCommand());
    }

#if CLIENT

    CGSelectionZone m_menu;
    CardVisual m_castCard;

    public override void ExecuteCommand()
    {
        GameObject castMenu = (GameObject) GameObject.Instantiate(Resources.Load("CardGame/Menus/SelectionMenu"));
        m_menu = castMenu.GetComponent<CGSelectionZone>();

        if(m_menu == null)
        {
            Debug.LogError("Cast Menu prefab missing CGCastMenuZone");
        }
        
        List<int> cardIDs = new List<int>();
        foreach(CastableCard card in m_castableCards)
        {
            cardIDs.Add(card.cardID);

            CardVisual cv = m_visualManager.GetCard(card.cardID);
            // Move card to CastMenu
            m_menu.MoveCardToZone(cv);
        }

        // Highlight all playable cards
        // Make playable cards clickable
        m_visualManager.MakeClickable(cardIDs, OnCardClick);
    }

    void OnCardClick(CardVisual card)
    {
        // Select the clicked card
        m_castCard = card;
        m_visualManager.MakeAllUnclickable();

        // Move the card to the targetting zone
        CGBasicZone targettingZone = m_visualManager.GetPlayerTargettingZone();
        CGChannelZone channelZero = m_visualManager.GetPlayerChannel(0);
        targettingZone.MoveCardToZone(card);

        // Close the card selection menu
        m_menu.Close(channelZero);

        // Highlight possible targets and wait for one to be picked
        List<int> targets = GetTargets(card);
        if (targets.Count == 0)
        {
            // No valid targets found, one of three possibilities:
            // - There may be no legal targets
            // - The spell may not target anything
            // - An error is causing no targets to be found
            // Tell the server to cast the spell with no targets and await a response
            SGC_CastSpellFromChannel0 command = new SGC_CastSpellFromChannel0(card.m_cardID, -1);
            m_visualManager.TransmitStream(command.PackCommand());
            FinishCommand();
            return;
        }
        else
        {
            m_visualManager.MakeClickable(targets, OnTargetClick);
        }
    }

    void OnTargetClick(CardVisual target)
    {
        m_visualManager.MakeAllUnclickable();
        SGC_CastSpellFromChannel0 command = new SGC_CastSpellFromChannel0(m_castCard.m_cardID, target.m_cardID);
        m_visualManager.TransmitStream(command.PackCommand());
        FinishCommand();
    }

    List<int> GetTargets(CardVisual card)
    {
        foreach(CastableCard targetInfo in m_castableCards)
        {
            if(targetInfo.cardID == card.m_cardID)
            {
                return targetInfo.targets;
            }
        }

        return new List<int>();
    }

#endif
}
