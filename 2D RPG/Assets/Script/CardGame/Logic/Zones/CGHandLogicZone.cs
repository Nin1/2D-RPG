using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGHandLogicZone : CGLogicZone {

    public CGHandLogicZone(CGPlayerLogic player, CardGameManager cgManager) : base(player, cgManager)
    {

    }

    protected override void OnCardEnter(CGCardObject card)
    {
        if (card.m_isSpell)
        {
            m_cgManager.AddEffectToStack(card.GetEffect(CGEffectType.ON_REMOVE));
            m_cgManager.TriggerEvent("SpellRemoved");

            //@TODO:
            //CGC_MoveCardToHand command = new CGC_MoveCardToHand(card.m_owner.m_ID, card.m_cardID);
            //m_cgManager.m_connection.TransmitStream(command.PackCommand(), 0);
            //m_cgManager.m_connection.TransmitStream(command.PackCommand(), 1);
        }
        card.SetIsSpell(false);
    }

    public CGCardObject GetRandomPlayableCard()
    {
        System.Random r = new System.Random();
        int startIndex = r.Next(0, m_cards.Count - 1);
        int i = startIndex;

        do
        {
            //if(CardGameManager.IsCardPlayable(m_cards[i])
            {
                return m_cards[i];
            }
            //else
            //i = (i + 1) % m_cards.Count;

        } while (i != startIndex);
    }

    public List<int> GetPlayableCardIDs()
    {
        List<int> playableCards = new List<int>();

        foreach(CGCardObject card in m_cards)
        {
            // if(CardGameManager.IsCardPlayable(card)
            playableCards.Add(card.m_cardID);
        }

        return playableCards;
    }
}
