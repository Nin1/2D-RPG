using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Fireball : CardScript {

    protected override void OnPlayScript()
    {
        CardGameTriggerManager.StartListening("BeginTurn", BeginTurn);
        Debug.Log("Fireball OnPlay resolved");
    }

    protected override void OnCastScript()
    {
        Debug.Log("Cast Fireball");
        DealDamageToOpponent(GetAttribute("Damage"));
    }

    protected override void OnCastCommand()
    {
        SendOpponentLifeUpdate();
    }

    protected override void OnRemoveScript()
    {
        CardGameTriggerManager.StopListening("BeginTurn", BeginTurn);
        Debug.Log("Fireball OnRemove resolved");
    }

    void BeginTurn()
    {
        Debug.Log("Fireball trigger at beginning of turn");
    }
}
