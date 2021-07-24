using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_FireBlast : CardScript
{
    protected override void OnCastScript()
    {
        int damage = GetAttribute("Damage");
        DealDamageToOpponent(damage);
        DealDamageToSelf(damage);
    }

    protected override void OnCastCommand()
    {
        SendOpponentLifeUpdate();
        SendSelfLifeUpdate();
    }
}
