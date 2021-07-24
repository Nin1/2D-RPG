using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_MindControl : CardScript
{
    protected override void OnCastScript()
    {
        m_target.SetController(m_caster);
    }

    protected override void OnCastCommand()
    {
        CGC_ChannelSpell command = new CGC_ChannelSpell(m_target.m_cardID, m_target.m_controller.m_ID, m_target.m_turnsRemaining);
        m_cgManager.m_connection.TransmitStream(command.PackCommand(), 0);
        m_cgManager.m_connection.TransmitStream(command.PackCommand(), 1);
    }

    public override bool CheckValidTarget(CGCardObject thisCard, CGCardObject target)
    {
        if(IsCardASpell(target))
        {
            if(target.m_controller != thisCard.m_controller)
            {
                return true;
            }
        }
        return false;
    }
}
