using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** CGLogicZone - A 'zone' in which cards can exist */
public class CGLogicZone {

    public List<CGCardObject> m_cards { get; private set; }

    protected CGPlayerLogic m_player;
    protected CardGameManager m_cgManager;

    public CGLogicZone(CGPlayerLogic player, CardGameManager cgManager)
    {
        m_cards = new List<CGCardObject>();
        m_player = player;
        m_cgManager = cgManager;
    }

    public void MoveCardToHere(CGCardObject card)
    {
        if (card.m_currentZone != null)
        {
            // Remove the card from its last zone's list
            card.m_currentZone.MoveCardFromZone(card);
            card.m_currentZone.OnCardExit(card);
        }

        card.m_currentZone = this;
        // Add the card to this zone's list
        m_cards.Add(card);
        OnCardEnter(card);

        return;
    }

    public int GetSize()
    {
        return m_cards.Count;
    }

    /** Called when a card moves from this zone to remove it from the cards list */
    private void MoveCardFromZone(CGCardObject card)
    {
        m_cards.Remove(card);
    }

    public CGCardObject GetCardWithID(int id)
    {
        foreach (CGCardObject card in m_cards)
        {
            if (card.m_cardID == id)
            {
                return card;
            }
        }
        return null;
    }

    public CGCardObject RemoveCardWithID(int cardID)
    {
        int i = 0;
        foreach (CGCardObject card in m_cards)
        {
            if (card.m_cardID == cardID)
            {
                break;
            }
            i++;
        }

        CGCardObject removedCard = m_cards[i];

        m_cards.RemoveAt(i);

        return removedCard;
    }

    protected virtual void OnCardEnter(CGCardObject card) { }
    protected virtual void OnCardExit(CGCardObject card) { }
}
