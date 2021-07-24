using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CGEffect = CardGameManager.CGEffect;

public class CardGameTriggerManager {

    Dictionary<string, List<CGEffect>> m_triggers = new Dictionary<string, List<CGEffect>>();

    private static CardGameTriggerManager m_triggerManager;

    private static CardGameTriggerManager instance
    {
        get
        {
            if(m_triggerManager == null)
            {
                m_triggerManager = new CardGameTriggerManager();
            }
            return m_triggerManager;
        }
    }

    public static void StartListening(string triggerName, CGEffect triggerFunc)
    {
        List<CGEffect> thisTrigger = null;
        if(instance.m_triggers.TryGetValue(triggerName, out thisTrigger))
        {
            thisTrigger.Add(triggerFunc);
        }
        else
        {
            thisTrigger = new List<CGEffect>();
            thisTrigger.Add(triggerFunc);
            instance.m_triggers.Add(triggerName, thisTrigger);
        }
    }

    public static void StopListening(string triggerName, CGEffect triggerFunc)
    {
        if (m_triggerManager == null)
            return;

        List<CGEffect> thisTrigger = null;
        if(instance.m_triggers.TryGetValue(triggerName, out thisTrigger))
        {
            thisTrigger.Remove(triggerFunc);
        }
    }

    public static List<CGEffect> GetTriggers(string triggerName)
    {
        List<CGEffect> thisTrigger = null;
        if (instance.m_triggers.TryGetValue(triggerName, out thisTrigger))
        {
            return thisTrigger;
        }
        return null;
    }
}
