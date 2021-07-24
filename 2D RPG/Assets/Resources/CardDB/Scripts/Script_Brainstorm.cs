using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Brainstorm : CardScript
{
    protected override void OnCastScript()
    {
        DealDamageToOpponent(m_cgManager.m_spellsCastThisTurn);
    }

    protected override void OnCastCommand()
    {
        SendOpponentLifeUpdate();
    }
}
