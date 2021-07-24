using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGDeckLogicZone : CGLogicZone
{
    public CGDeckLogicZone(CGPlayerLogic player, CardGameManager cgManager) : base(player, cgManager)
    {

    }

    protected override void OnCardEnter(CGCardObject card)
    {
        if (card.m_isSpell)
        {
            m_cgManager.AddEffectToStack(card.GetEffect(CGEffectType.ON_REMOVE));
            m_cgManager.TriggerEvent("SpellRemoved");

            // @TODO:
            //CGC_MoveCardToDeck command = new CGC_MoveCardToDeck(card.m_owner.m_ID, card.m_cardID);
            //m_cgManager.m_connection.TransmitStream(command.PackCommand(), 0);
            //m_cgManager.m_connection.TransmitStream(command.PackCommand(), 1);
        }
        card.SetIsSpell(false);
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
