using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_FocusBlast : CardScript
{
    protected override void OnCastScript()
    {
        int damage = m_caster.m_spells.GetSize();
        DealDamageToOpponent(damage);
    }

    protected override void OnCastCommand()
    {
        SendOpponentLifeUpdate();
    }
}
