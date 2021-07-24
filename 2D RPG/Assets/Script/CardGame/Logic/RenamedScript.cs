using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CGEffect = CardGameManager.CGEffect;

public enum CGEffectType
{
    ON_CAST = 0,
    ON_CHANNEL,
    ON_PLAY,
    ON_REMOVE,
}

public class CGCardObject {

    public int m_cardID { get; private set; }
    public string m_cardName { get; private set; }
    public int m_channelCost { get; private set; }
    public CardType m_cardType { get; private set; }
    public List<CardAttribute> m_attributes { get; private set; }
    public CardScript m_script { get; private set; }
    public CardData m_data { get; private set; }
    public CGPlayerLogic m_owner { get; private set; }
    public CGPlayerLogic m_controller { get; private set; }
    public int m_turnsRemaining { get; private set; }
    public CGLogicZone m_currentZone;
    public bool m_isSpell { get; private set; }

    public CGCardObject(CardData data, CardGameManager cgManager, CGPlayerLogic owner)
    {
        // Load card object data from card data
        m_cardName = data.cardName;
        m_channelCost = data.channelCost;
        m_attributes = data.cardAttributes;
        m_turnsRemaining = m_channelCost;

        // Create an instance of the card script
        if (data.cardScriptName == "")
        {
            Debug.LogError("No script specified for card: " + m_cardName);
        }
        else
        {
            m_script = System.Activator.CreateInstance(System.Type.GetType(data.cardScriptName), new object[] { }) as CardScript;
            m_script.SetScriptData(data, cgManager, this);
        }

        m_owner = owner;
        m_controller = owner;

        // Create unique card ID
        m_cardID = m_allCardIDs.Count;
        m_allCardIDs.Add(m_allCardIDs.Count);

        m_data = data;
    }
    
    public CGEffect GetEffect(CGEffectType effectType)
    {
        switch (effectType)
        {
            case CGEffectType.ON_CAST:
                return m_script.OnCast;
            case CGEffectType.ON_CHANNEL:
                return m_script.OnChannel;
            case CGEffectType.ON_PLAY:
                return m_script.OnPlay;
            case CGEffectType.ON_REMOVE:
                return m_script.OnRemove;
            default:
                return null;
        }
    }

    public void SetIsSpell(bool isSpell)
    {
        m_isSpell = isSpell;
    }

    public void SetController(CGPlayerLogic player)
    {
        m_controller = player;
        player.m_spells.MoveCardToHere(this);
    }

    /************************************
     * CardScript behaviour passthrough *
     ************************************/

    public void PlayFromHand(CGPlayerLogic player)
    {
        m_script.PlayFromHand(this, player);
    }

    /** "Channel" the spell (in most cases, decrease its time remaining by 1) */
    public void ChannelSpell()
    {
        m_script.ChannelSpell(this);
    }

    public void Cast()
    {
        m_script.CastSpell(m_controller);
    }

    public void Remove()
    {
        m_script.RemoveSpell(this, m_owner);
    }

    public void SetTarget(CGCardObject target)
    {
        m_script.SetTarget(target);
    }

    public void IncreaseTimeRemaining(int amount)
    {
        m_turnsRemaining += amount; // @TODO: Check if greater than 9 and discard 
        if(m_turnsRemaining > 9)
        {
            Remove();
        }
    }

    public void DecreaseTimeRemaining(int amount)
    {
        m_turnsRemaining -= amount;
        if(m_turnsRemaining < 0)
        {
            m_turnsRemaining = 0;
        }
    }

    public void SetTimeRemaining(int turns)
    {
        m_turnsRemaining = turns;
    }

    public int GetTimeRemaining()
    {
        return m_turnsRemaining;
    }

    static List<int> m_allCardIDs = new List<int>();
}
