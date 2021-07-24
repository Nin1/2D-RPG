using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CGPlayerLogic {

    // Members:
    public CGHandLogicZone m_hand { get; private set; }
    public CGDeckLogicZone m_deck { get; private set; }
    public CGSpellLogicZone m_spells { get; private set; }
    public CGGraveyardLogicZone m_graveyard { get; private set; }

    public int m_ID { get; private set; }
    CardGameManager m_cgManager;
    public bool m_hasDeck { get; private set; }

    public int m_life { get; private set; }

    public CGPlayerLogic(CardGameManager cgManager, int playerID)
    {
        m_hand = new CGHandLogicZone(this, cgManager);
        m_deck = new CGDeckLogicZone(this, cgManager);
        m_spells = new CGSpellLogicZone(this, cgManager);
        m_graveyard = new CGGraveyardLogicZone(this, cgManager);
        m_cgManager = cgManager;
        m_ID = playerID;
        m_hasDeck = false;

        FillDeckRandom();
    }

    /** Fill the deck with random cards */
    void FillDeckRandom()
    {
        string[] fileList = Directory.GetFiles(CardData.cardJSONPath, "*.json");

        List<CardData> cards = new List<CardData>();

        foreach(string path in fileList)
        {
            cards.Add(CardData.LoadCardData(path));
        }

        System.Random r = new System.Random((int)(DateTime.Now.Millisecond + (m_ID * 1000)));
        CardData[] deck = new CardData[30];

        for (int i = 0; i < 30; i++)
        {
            int randomIndex = r.Next(0, cards.Count - 1);
            deck[i] = cards[randomIndex];
            Debug.Log("Added " + deck[i].cardName + " to deck");
        }

        m_deck.Populate(deck);
    }

    public void AssignDeck(PackedDeck packedDeck)
    {
        // TODO: A CardProvider class to load and cache CardData instead of loading here every time.
        //       That way it can correctly sort into IDs instead of sorting by whatever order they load in.
        // Load all the cards (eyeroll)
        List<CardData> cards = new List<CardData>();
        string[] fileList = Directory.GetFiles(CardData.cardJSONPath, "*.json");
        foreach (string path in fileList)
        {
            cards.Add(CardData.LoadCardData(path));
        }

        // Build the deck from the card IDs in the PackedDeck
        CardData[] deck = new CardData[30];
        for(int i = 0; i < packedDeck.cardIDs.Count; ++i)
        {
            deck[i] = cards[packedDeck.cardIDs[i]];
        }
        m_deck.Populate(deck);
        m_hasDeck = true;
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
                // Tell player 0 what card they drew
                CGC_PlayerDrawCard command = new CGC_PlayerDrawCard(card.m_data, card.m_cardID);
                m_cgManager.m_connection.TransmitStream(command.PackCommand(), 0);
                // Tell player 1 that player 0 drew a card
                CGC_OpponentDrawCard oppCommand = new CGC_OpponentDrawCard();
                m_cgManager.m_connection.TransmitStream(oppCommand.PackCommand(), 1);
            }
            else
            {
                // Tell player 1 what card they drew
                CGC_PlayerDrawCard command = new CGC_PlayerDrawCard(card.m_data, card.m_cardID);
                m_cgManager.m_connection.TransmitStream(command.PackCommand(), 1);
                // Tell player 0 that player 1 drew a card
                CGC_OpponentDrawCard oppCommand = new CGC_OpponentDrawCard();
                m_cgManager.m_connection.TransmitStream(oppCommand.PackCommand(), 0);
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
                m_cgManager.m_connection.TransmitStream(command.PackCommand(), 0);
                CGC_OpponentPlayCardFromHand oppCommand = new CGC_OpponentPlayCardFromHand(card.m_data, card.m_cardID, card.GetTimeRemaining());
                m_cgManager.m_connection.TransmitStream(oppCommand.PackCommand(), 1);
            }
            else
            {
                CGC_PlayerPlayCardFromHand command = new CGC_PlayerPlayCardFromHand(card.m_data, card.m_cardID, card.GetTimeRemaining());
                m_cgManager.m_connection.TransmitStream(command.PackCommand(), 1);
                CGC_OpponentPlayCardFromHand oppCommand = new CGC_OpponentPlayCardFromHand(card.m_data, card.m_cardID, card.GetTimeRemaining());
                m_cgManager.m_connection.TransmitStream(oppCommand.PackCommand(), 0);
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
                m_cgManager.m_connection.TransmitStream(command.PackCommand(), 0);
                CGC_OpponentPlayCardFromHand oppCommand = new CGC_OpponentPlayCardFromHand(card.m_data, card.m_cardID, card.GetTimeRemaining());
                m_cgManager.m_connection.TransmitStream(oppCommand.PackCommand(), 1);
            }
            else
            {
                CGC_PlayerPlayCardFromHand command = new CGC_PlayerPlayCardFromHand(card.m_data, card.m_cardID, card.GetTimeRemaining());
                m_cgManager.m_connection.TransmitStream(command.PackCommand(), 1);
                CGC_OpponentPlayCardFromHand oppCommand = new CGC_OpponentPlayCardFromHand(card.m_data, card.m_cardID, card.GetTimeRemaining());
                m_cgManager.m_connection.TransmitStream(oppCommand.PackCommand(), 0);
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
        m_cgManager.m_connection.TransmitStream(command.PackCommand(), 0);
        m_cgManager.m_connection.TransmitStream(command.PackCommand(), 1);
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
                m_graveyard.MoveCardToHere(spell);

                return;
            }
        }

    }
}
