using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_BlessingOfVigour : CardScript
{
    protected override void OnCastScript()
    {
        Lifegain();
    }

    protected override void OnCastCommand()
    {
        SendSelfLifeUpdate();
    }
}
