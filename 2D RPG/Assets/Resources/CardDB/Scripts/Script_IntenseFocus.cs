using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_IntenseFocus : CardScript
{
    protected override void OnCastScript()
    {
        m_cgManager.AlterSpellDamageMultiplier(2);
    }
}
