using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGPlayerLogic {

    // Members:
    public int m_lifeTotal { get { return m_lifeTotal; } private set { } }
    public CGHandLogicZone m_hand { get; private set; }
    public CGDeckLogicZone m_deck { get; private set; }
    public CGSpellLogicZone m_spells { get; private set; }
    public CGLogicZone m_graveyard { get; private set; }

    public int m_ID { get; private set; }
    CardGameManager m_cgManager;

    public int m_life { get; private set; }

    public CGPlayerLogic(CardGameManager cgManager, int playerID)
    {
        m_hand = new CGHandLogicZone(this, cgManager);
        m_deck = new CGDeckLogicZone(this, cgManager);
        m_spells = new CGSpellLogicZone(this, cgManager);
        m_graveyard = new CGLogicZone(this, cgManager);
        m_cgManager = cgManager;
        m_ID = playerID;

        CardData[] deck = new CardData[30];
        Object[] allCards = Resources.LoadAll("CardDB/Cards");

        for(int i = 0; i < 30; i++)
        {
            int randomIndex = Random.Range(0, allCards.Length);
            deck[i] = (CardData)allCards[randomIndex];
            //Debug.Log("Added " + deck[i].cardName + " to deck");
        }
        m_deck.Populate(deck);
    }

    // Functions
    public void DrawCards(int numCards)
    {
        Debug.Log("Drawing " + numCards + " cards");

        for (int i = 0; i < numCards; i++)
        {
            CGCardObject card = m_deck.GetTopCard();
            if (card == null)
            {
                // This player loses the game
                return;
            }
            
            m_hand.MoveCardToHere(card);

            // Create the 'card draw' client command
            if (m_ID == 0)
            {
                CGC_PlayerDrawCard command = new CGC_PlayerDrawCard(card.m_data, card.m_cardID);
                CGVisualManager.instance.AddCommand(command);
            }
            else
            {
                CGC_OpponentDrawCard command = new CGC_OpponentDrawCard();
                CGVisualManager.instance.AddCommand(command);
            }
        }
    }

    public void DiscardCard(int cardID)
    {

    }

    public int GetCardsInHand()
    {
        return m_hand.GetSize();
    }

    public bool CanPlayCard(int cardID)
    {
        CGCardObject card = m_hand.GetCardWithID(cardID);
        if (card != null)
        {
            //if (CardGameManager.IsCardPlayable(card)
            return true;
        }
        return false;
    }

    /** Create a spell from the card in the player's hand, adding any "OnPlay" effect to the stack */
    public void PlayCardFromHand(int cardID)
    {
        // Create a spell from the chosen card
        CGCardObject card = m_hand.GetCardWithID(cardID);
        if(card != null)
        {
            card.PlayFromHand(this);

            if (m_ID == 0)
            {
                CGC_PlayerPlayCardFromHand command = new CGC_PlayerPlayCardFromHand(card.m_data, card.m_cardID, card.GetTimeRemaining());
                CGVisualManager.instance.AddCommand(command);
            }
            else
            {
                CGC_OpponentPlayCardFromHand command = new CGC_OpponentPlayCardFromHand(card.m_data, card.m_cardID, card.GetTimeRemaining());
                CGVisualManager.instance.AddCommand(command);
            }

            Debug.Log("Played card: " + card.m_cardName);

            // Add any "OnPlay" effect to the stack
            m_cgManager.AddEffectToStack(card.GetEffect(CGEffectType.ON_PLAY));
        }
    }

    public void CastSpell(CGCardObject card)
    {
        card.Cast();
    }

    public void PlayRandomCardFromHand()
    {
        CGCardObject card = m_hand.GetRandomPlayableCard();
        if (card != null)
        {
            card.PlayFromHand(this);

            if (m_ID == 0)
            {
                CGC_PlayerPlayCardFromHand command = new CGC_PlayerPlayCardFromHand(card.m_data, card.m_cardID, card.GetTimeRemaining());
                CGVisualManager.instance.AddCommand(command);
            }
            else
            {
                CGC_OpponentPlayCardFromHand command = new CGC_OpponentPlayCardFromHand(card.m_data, card.m_cardID, card.GetTimeRemaining());
                CGVisualManager.instance.AddCommand(command);
            }

            Debug.Log("Played card: " + card.m_cardName);

            // Add any "OnPlay" effect to the stack
            m_cgManager.AddEffectToStack(card.GetEffect(CGEffectType.ON_PLAY));
        }
    }

    /** Return the actual life lost */
    public int LoseLife(int amount)
    {
        amount *= m_cgManager.m_spellDamageMultiplier;
        m_life -= amount;
        return amount;
    }

    public void GainLife(int amount)
    {
        m_life += amount;
    }

    public void SetLife(int amount)
    {
        m_life = amount;
    }

    /** Tell the visuals to update the life total
     *  @TODO: Probably don't do this here, do it during an effect's animation in the future */
    public void SendLifeCommand()
    {
        CGC_SetLife command = new CGC_SetLife(m_life, m_ID);
        CGVisualManager.instance.AddCommand(command);
    }

    public void ChannelAllSpells()
    {

    }

    public List<CGCardObject> GetSpells()
    {
        return m_spells.m_cards;
    }

    public CGCardObject GetSpell(int cardID)
    {
        foreach(CGCardObject spell in GetSpells())
        {
            if(spell.m_cardID == cardID)
            {
                return spell;
            }
        }

        return null;
    }

    public void RemoveSpell(int spellID)
    {
        // Find the spell with the spell ID
        foreach(CGCardObject spell in GetSpells())
        {
            if(spell.m_cardID == spellID)
            {
                // Add OnRemove to stack
                m_cgManager.AddEffectToStack(spell.GetEffect(CGEffectType.ON_REMOVE));
                m_cgManager.TriggerEvent("SpellRemoved");

                // If the spell has a card, put it in the graveyard
                m_graveyard.MoveCardToHere(spell);

                return;
            }
        }

    }
}
