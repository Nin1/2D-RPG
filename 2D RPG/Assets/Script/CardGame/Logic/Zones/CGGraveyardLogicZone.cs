using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGGraveyardLogicZone : CGLogicZone
{
    public CGGraveyardLogicZone(CGPlayerLogic player, CardGameManager cgManager) : base(player, cgManager)
    {

    }

    protected override void OnCardEnter(CGCardObject card)
    {
        if (card.m_isSpell)
        {
            m_cgManager.AddEffectToStack(card.GetEffect(CGEffectType.ON_REMOVE));
            m_cgManager.TriggerEvent("SpellRemoved");

            CGC_MoveCardToGraveyard command = new CGC_MoveCardToGraveyard(card.m_owner.m_ID, card.m_cardID);
            m_cgManager.m_connection.TransmitStream(command.PackCommand(), 0);
            m_cgManager.m_connection.TransmitStream(command.PackCommand(), 1);
        }
        card.SetIsSpell(false);
    }
}
