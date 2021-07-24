using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Firebolt : CardScript
{
    protected override void OnCastScript()
    {
        DealDamageToOpponent(GetAttribute("Damage"));
    }

    protected override void OnCastCommand()
    {
        SendOpponentLifeUpdate();
    }
}
