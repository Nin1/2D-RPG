using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGSpellLogicZone : CGLogicZone
{
    public CGSpellLogicZone(CGPlayerLogic player, CardGameManager cgManager) : base(player, cgManager)
    {

    }

    protected override void OnCardExit(CGCardObject card)
    {
        m_cgManager.AddEffectToStack(card.GetEffect(CGEffectType.ON_REMOVE));
    }
}
