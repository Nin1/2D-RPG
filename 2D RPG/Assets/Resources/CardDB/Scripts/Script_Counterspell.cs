using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Counterspell : CardScript
{
    protected override void OnCastScript()
    {
        Debug.Log("Cast Counterspell");
        m_target.Remove();
    }

    public override bool CheckValidTarget(CGCardObject thisCard, CGCardObject target)
    {
        return IsCardASpell(target);
    }
}
