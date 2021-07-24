using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGHandLogicZone : CGLogicZone {

    public CGHandLogicZone(CGPlayerLogic player, CardGameManager cgManager) : base(player, cgManager)
    {

    }

    public CGCardObject GetRandomPlayableCard()
    {
        int startIndex = (int)UnityEngine.Random.Range(0, m_cards.Count - 0.0001f);
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
