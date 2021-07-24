using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CGEffect = CardGameManager.CGEffect;


public class CGSpellObject {
    // @TODO: Remove this class, use CGCardObject for everything
    public int m_spellID { get; private set; }
    public int m_turnsRemaining { get; private set; }
    public CGCardObject m_card { get; private set; }
    public int m_playerID { get; private set; }

    public CGSpellObject(CGCardObject card, int playerID)
    {
        m_turnsRemaining = card.m_channelCost;
        m_card = card;
        m_playerID = playerID;

        m_spellID = m_allSpellIDs.Count;
        m_allSpellIDs.Add(m_spellID);
    }

    public CGEffect GetEffect(CGEffectType effectType)
    {
        switch(effectType)
        {
            case CGEffectType.ON_CAST:
                return m_card.m_script.OnCast;
            case CGEffectType.ON_CHANNEL:
                return m_card.m_script.OnChannel;
            case CGEffectType.ON_PLAY:
                return m_card.m_script.OnPlay;
            case CGEffectType.ON_REMOVE:
                return m_card.m_script.OnRemove;
            default:
                return null;
        }
    }

    public void DecreaseTimeRemaining(int amount)
    {
        Debug.Log("Decreasing channel on spell " + m_card.m_cardName);
        m_turnsRemaining -= amount;
    }

    public int GetTimeRemaining()
    {
        return m_turnsRemaining;
    }

    static List<int> m_allSpellIDs = new List<int>();
}
