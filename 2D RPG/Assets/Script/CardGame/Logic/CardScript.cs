using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardScript {

    protected CardData m_data;
    protected CardGameManager m_cgManager;
    protected List<CardAttribute> m_attributes;

    /** The card that owns this script */
    protected CGCardObject m_card;

    protected CGCardObject m_target;

    /** The player who played this card from their hand */
    protected CGPlayerLogic m_owner;

    /** The player who is casting this card in their turn */
    protected CGPlayerLogic m_caster;

    public void SetScriptData(CardData data, CardGameManager cgManager, CGCardObject card)
    {
        m_data = data;
        m_cgManager = cgManager;
        m_card = card;
        m_attributes = m_data.cardAttributes;
    }
    /** Sets the target of the card, if any */
    public void SetTarget(CGCardObject target)
    {
        m_target = target;
    }
    /** A check on a given card, returning true if it is a valid target */
    public virtual bool CheckValidTarget(CGCardObject thisCard, CGCardObject target) { return false; }

    /************************
     ** DEFAULT BEHAVIOURS **
     ************************/

    /** Called when playing the card from hand to put the spell into play */
    public void PlayFromHand(CGCardObject card, CGPlayerLogic player)
    {
        m_owner = player;

        card.SetTimeRemaining(m_data.channelCost);
        player.m_spells.MoveCardToHere(card);
        m_cgManager.AddEffectToStack(OnPlay);
    }

    /** Called at the start of the "Channel phase" to decrease the time remaining on the spell */
    public void ChannelSpell(CGCardObject card)
    {
        card.DecreaseTimeRemaining(1);
        m_cgManager.AddEffectToStack(OnChannel);
    }

    /** Called when the spell is to be cast */
    public void CastSpell(CGPlayerLogic caster)
    {
        m_cgManager.AddEffectToStack(OnCast);
        m_caster = caster;
    }
    
    /** Called when the spell is to be removed from the board */
    public void RemoveSpell(CGCardObject card, CGPlayerLogic owner)
    {
        owner.m_graveyard.MoveCardToHere(card);
    }

    /***************
     ** CGEffects **
     ***************/

    // These are called by the stack in CardGameManager to execute the effect

    public void OnPlay()
    {
        OnPlayScript();
        OnPlayCommand();
    }

    public void OnCast()
    {
        OnCastScript();

        CGC_CastSpell command = new CGC_CastSpell(m_card.m_cardID, m_caster.m_ID);
        m_cgManager.m_connection.TransmitStream(command.PackCommand(), 0);
        m_cgManager.m_connection.TransmitStream(command.PackCommand(), 1);

        OnCastCommand();
    }

    public void OnChannel()
    {
        OnChannelScript();
        OnChannelCommand();
    }

    public void OnRemove()
    {
        OnRemoveScript();
        OnRemoveCommand();
    }

    /** A continuous effect applied every time something happens */
    public virtual void ContinuousScript() { }

    /********************
     ** EFFECT SCRIPTS **
     ********************/

    // These are the scripts to make the card do what it says

    /** When the card is played from hand */
    protected virtual void OnPlayScript() { }
    /** When the spell is cast at stack 0 */
    protected virtual void OnCastScript() { }
    /** When the spell is moved stack by the channel phase */
    protected virtual void OnChannelScript() { }
    /** When the spell is removed from the game (e.g. after casting, being countered) */
    protected virtual void OnRemoveScript() { }

    /*********************
     ** VISUAL COMMANDS **
     *********************/

    // These functions send the visual command to the client when the effect is run 

    protected virtual void OnPlayCommand() { }

    protected virtual void OnCastCommand()
    {
    }

    protected virtual void OnChannelCommand() { }

    protected virtual void OnRemoveCommand()
    {
    }
    
    /********************
     ** COMMON EFFECTS **
     ********************/
    
    /** Deal damage to your opponent. Returns the actual damage done. */
    protected int DealDamageToOpponent(int damage)
    {
        int casterID = m_caster.m_ID;
        int targetID = (casterID + 1) % 2;

        CGPlayerLogic target = m_cgManager.m_players[targetID];
        return target.LoseLife(damage);
    }

    protected int DealDamageToSelf(int damage)
    {
        return m_caster.LoseLife(damage);
    }

    protected void GainLife(int amount)
    {
        m_caster.GainLife(amount);
    }

    /** Gain life equal to the "Lifegain" attribute of the card */
    protected void Lifegain()
    {
        int amount = GetAttribute("Lifegain");
        m_caster.GainLife(amount);
    }

    protected void SendOpponentLifeUpdate()
    {
        int casterID = m_caster.m_ID;
        int opponentID = (casterID + 1) % 2;

        CGPlayerLogic opponent = m_cgManager.m_players[opponentID];
        opponent.SendLifeCommand();
    }

    protected void SendSelfLifeUpdate()
    {
        m_caster.SendLifeCommand();
    }

    protected int GetAttribute(string name)
    {
        CardAttribute attribute = m_attributes.Find(attrib => attrib.name == name);
        if (attribute != null)
        {
            return attribute.value;
        }
        return 0;
    }

    protected bool IsCardASpell(CGCardObject card)
    {
        if (card.m_isSpell)
        {
            return true;
        }
        return false;
    }
}