using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGDeckLogicZone : CGLogicZone
{
    public CGDeckLogicZone(CGPlayerLogic player, CardGameManager cgManager) : base(player, cgManager)
    {

    }

    public void Populate(CardData[] cards)
    {
        foreach (CardData data in cards)
        {
            CGCardObject card = new CGCardObject(data, m_cgManager, m_player);
            MoveCardToHere(card);
        }
        Shuffle();
    }
    
    public void Shuffle()
    {
        // @TODO
    }

    public CGCardObject GetTopCard()
    {
        if (m_cards.Count > 0)
        {
            CGCardObject card = m_cards[0];
            return card;
        }
        return null;
    }
}
