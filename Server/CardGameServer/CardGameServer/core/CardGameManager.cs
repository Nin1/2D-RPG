﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CGTurnState
{
    DEFAULT = 0,
    WAIT_FOR_PLAY_CARD_FROM_HAND,
    CARD_PLAYED_FROM_HAND,
    WAIT_FOR_CAST_ORDER,
    ALL_SPELLS_CAST,
    WAIT_FOR_TURN_END
}

public enum CGPhase
{
    BEGINNING = 0,
    CHANNEL,
    DRAW,
    PLAY,
    CAST,
    END
}

public class CardGameManager : MonoBehaviour {

    public static CardGameManager instance;

    public delegate void CGEffect();
    public List<CGPlayerLogic> m_players { get; private set; }

    /** Per-turn-resetting variables */
    //@TODO: Make a proper system for these
    public int m_spellsCastThisTurn { get; private set; }
    public int m_spellDamageMultiplier { get; private set; }

    int m_activePlayerID = 1;
    CGPlayerLogic m_activePlayer;
    bool m_gameInProgress = true;
    List<CGEffect> m_stack = new List<CGEffect>();
    CGTurnState m_turnState = CGTurnState.DEFAULT;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CGPlayerLogic player1 = new CGPlayerLogic(this, 0);
        CGPlayerLogic player2 = new CGPlayerLogic(this, 1);
        m_players = new List<CGPlayerLogic>();
        m_players.Add(player1);
        m_players.Add(player2);
        m_activePlayer = player1;
        StartGame();
    }

    /** Single turn loop */
    void Update()
    {
    }

    void StartGame()
    {
        foreach(CGPlayerLogic player in m_players)
        {
            player.DrawCards(3);
            player.GainLife(20);
            player.SendLifeCommand();
        }
        RunGameLogic();
    }

    /****************
     * Flow Control *
     ****************/

    void RunGameLogic()
    {
        while (m_gameInProgress)
        {
            if (m_turnState == CGTurnState.DEFAULT)
            {
                m_turnState = Phase0();   // Beginning
                m_turnState = Phase1();   // Channel
                m_turnState = Phase2();   // Draw
                m_turnState = Phase3(); // Play
                if (m_turnState == CGTurnState.WAIT_FOR_PLAY_CARD_FROM_HAND)
                {
                    return; // Wait for the player to play a card from their hand
                }
            }
            if (m_turnState == CGTurnState.CARD_PLAYED_FROM_HAND || m_turnState == CGTurnState.DEFAULT)
            {
                m_turnState = Phase4(); // Cast
                if (m_turnState == CGTurnState.WAIT_FOR_CAST_ORDER)
                {
                    if (m_activePlayer == m_players[1])
                    {
                        // Tell AI to play all their spells
                        m_turnState = TellAIToCastAllSpells();
                    }
                    else
                    {
                        return; // Wait for the player to cast a spell from channel 0
                    }
                }
            }
            if (m_turnState == CGTurnState.ALL_SPELLS_CAST || m_turnState == CGTurnState.DEFAULT)
            {
                m_turnState = Phase5();
            }
        }
    }

    // Client-called function to play a card from the player's hand
    public void PlayCardFromHand(int player, int cardID)
    {
        if (m_turnState == CGTurnState.WAIT_FOR_PLAY_CARD_FROM_HAND && m_activePlayerID == player)
        {
            if(m_activePlayer.CanPlayCard(cardID))
            {
                // Play the card from hand, adding any "OnPlay" effects to the stack
                m_activePlayer.PlayCardFromHand(cardID);

                // Add any "PlayCard" triggered effects to the stack
                TriggerEvent("PlayCard");

                ResolveStack();

                // Return to game logic once a card has been played
                m_turnState = CGTurnState.CARD_PLAYED_FROM_HAND;
                RunGameLogic();
                return;
            }

            // Tell client to highlight playable cards and keep waiting for a valid card
            List<int> playableCardIDs = m_activePlayer.m_hand.GetPlayableCardIDs();
            CGC_WaitForPlayFromHand command = new CGC_WaitForPlayFromHand(playableCardIDs, m_activePlayerID);
            CGVisualManager.instance.AddCommand(command);
        }
    }

    public void PlayCardForAI()
    {
        if (m_turnState == CGTurnState.WAIT_FOR_PLAY_CARD_FROM_HAND && m_activePlayerID != CGVisualManager.instance.m_playerID)
        {
            // Play the card from hand, adding any "OnPlay" effects to the stack
            m_activePlayer.PlayRandomCardFromHand();

            // Add any "PlayCard" triggered effects to the stack
            TriggerEvent("PlayCard");

            ResolveStack();

            // Return to game logic once a card has been played
            m_turnState = CGTurnState.CARD_PLAYED_FROM_HAND;
            RunGameLogic();
            return;
        }
    }

    // Client-called function to skip playing a card from the player's hand
    public void SkipCardFromHand(int player)
    {
        if (m_activePlayerID == player && m_turnState == CGTurnState.WAIT_FOR_PLAY_CARD_FROM_HAND)
        {
            TriggerEvent("SkipCard");

            ResolveStack();

            m_turnState = CGTurnState.CARD_PLAYED_FROM_HAND;
            RunGameLogic();
            return;
        }
    }

    public void CastSpellFromChannel0(int spellCardID, int targetCardID)
    {
        // Ideally this would only listen to requests from the active player

        if(m_turnState == CGTurnState.WAIT_FOR_CAST_ORDER)
        {
            CGCardObject spell = m_activePlayer.GetSpell(spellCardID);
            if (spell == null)
            {
                Debug.LogError("Requested spell not found!");
                m_turnState = TellClientToCastSpell();
                return;
            }
            if (!spell.m_data.hasTargets)
            {
                // Cast spell with no targets
                CastSpell(spell);
            }
            else if (!SpellHasValidTargets(spell))
            {
                // Spell with no valid targets fizzles
                FizzleSpell(spell);
            }
            else if (IsTargetValid(spell, targetCardID))
            {
                // Cast spell with target
                CGCardObject target = FindCard(targetCardID);
                if (target != null)
                {
                    spell.SetTarget(target);
                }
                CastSpell(spell);
            }
            else 
            {
                Debug.LogError("Requested spell not given valid target when at least one exists");
                m_turnState = TellClientToCastSpell();
                return;
            }

            // Spell has been cast, continue gameplay
            if (m_activePlayerID == 0)   // @TODO: Update this for multiplayer
            {
                m_turnState = TellClientToCastSpell();

                if (m_turnState == CGTurnState.ALL_SPELLS_CAST)
                {
                    RunGameLogic();
                }
            }
        }
    }

    void CastSpell(CGCardObject spell)
    {
        spell.Cast();
        m_spellsCastThisTurn++;
        ResolveStack();
        spell.Remove();
        ResolveStack();
    }

    void FizzleSpell(CGCardObject spell)
    {
        // CGC_FizzleSpell(spell.m_cardID);
        m_spellsCastThisTurn++;
        spell.Remove();
        ResolveStack();
    }

    /*************
     * Targeting *
     *************/

    List<int> GetAllValidTargetsForSpell(CGCardObject spell)
    {
        List<int> targetIDs = new List<int>();

        // Check every card in every zone for targets

        int controllerID = spell.m_controller.m_ID;
        int opponentID = (controllerID + 1) % 2;

        // Controller's hand
        foreach(CGCardObject target in m_players[controllerID].m_hand.m_cards)
        {
            if(target != spell && spell.m_script.CheckValidTarget(spell, target))
            {
                targetIDs.Add(target.m_cardID);
            }
        }

        // Controller's spells
        foreach (CGCardObject target in m_players[controllerID].m_spells.m_cards)
        {
            if (target != spell && spell.m_script.CheckValidTarget(spell, target))
            {
                targetIDs.Add(target.m_cardID);
            }
        }

        // Opponent's spells
        foreach (CGCardObject target in m_players[opponentID].m_spells.m_cards)
        {
            if (target != spell && spell.m_script.CheckValidTarget(spell, target))
            {
                targetIDs.Add(target.m_cardID);
            }
        }

        return targetIDs;
    }

    bool IsTargetValid(CGCardObject spell, int targetCardID)
    {
        List<int> targets = GetAllValidTargetsForSpell(spell);
        return targets.Contains(targetCardID);
    }

    bool SpellHasValidTargets(CGCardObject spell)
    {
        List<int> targets = GetAllValidTargetsForSpell(spell);
        return targets.Count > 0;
    }

    CGCardObject FindCard(int cardID)
    {
        // Search all card zones for the card

        // Player 1's hand
        foreach (CGCardObject card in m_players[0].m_hand.m_cards)
        {
            if (card.m_cardID == cardID)
            {
                return card;
            }
        }

        // Player 2's hand
        foreach (CGCardObject card in m_players[1].m_hand.m_cards)
        {
            if (card.m_cardID == cardID)
            {
                return card;
            }
        }

        // Player 1's spells
        foreach (CGCardObject card in m_players[0].m_spells.m_cards)
        {
            if (card.m_cardID == cardID)
            {
                return card;
            }
        }

        // Player 2's spells
        foreach (CGCardObject card in m_players[1].m_spells.m_cards)
        {
            if (card.m_cardID == cardID)
            {
                return card;
            }
        }

        return null;
    }

    /*********************
     * Effect Resolution *
     *********************/

    public void AddEffectToStack(CGEffect effect)
    {
        m_stack.Add(effect);
    }

    void ResolveStack()
    {
        while(m_stack.Count > 0)
        {
            ResolveTopOfStack();
            //ApplyContinuousEffects();
            //CheckStateBasedActions();
        }
    }

    void ResolveTopOfStack()
    {
        Debug.Log("Resolving top of stack");
        CGEffect effect = m_stack[0];
        effect();
        Debug.Log("Removing effect from stack");
        m_stack.RemoveAt(0);
    }

    /** Add all triggered effects from this trigger to the stack */
    public void TriggerEvent(string triggerName)
    {
        Debug.Log(triggerName + " triggered");
        List<CGEffect> triggerEffects = CardGameTriggerManager.GetTriggers(triggerName);
        if (triggerEffects != null)
        {
            foreach (CGEffect trigger in triggerEffects)
            {
                AddEffectToStack(trigger);
                Debug.Log("Adding trigger to stack");
            }
        }
    }

    public void AlterSpellDamageMultiplier(int factor)
    {
        m_spellDamageMultiplier *= factor;
    }

    /***************
     * Turn phases *
     ***************/

    /** "Beginning of turn" phase */
    CGTurnState Phase0()
    {
        Debug.Log("Phase 0: Beginning phase");
        m_activePlayerID = (m_activePlayerID + 1) % m_players.Count;
        m_activePlayer = m_players[m_activePlayerID];
        m_spellsCastThisTurn = 0;
        m_spellDamageMultiplier = 1;
        TriggerEvent("BeginTurn");

        ResolveStack();

        return CGTurnState.DEFAULT;
    }

    /** "Channel" phase */
    CGTurnState Phase1()
    {
        Debug.Log("Phase 1: Channel phase");
        List<SpellChannelData> commandData = new List<SpellChannelData>();

        foreach (CGCardObject card in m_activePlayer.GetSpells())
        {
            card.ChannelSpell();

            SpellChannelData data = new SpellChannelData();
            data.cardID = card.m_cardID;
            data.playerID = m_activePlayerID;
            data.newChannel = card.GetTimeRemaining();
            commandData.Add(data);
        }
        
        CGC_ChannelSpell command = new CGC_ChannelSpell(commandData);
        CGVisualManager.instance.AddCommand(command);

        ResolveStack();

        return CGTurnState.DEFAULT;
    }

    /** "Draw" phase */
    CGTurnState Phase2()
    {
        Debug.Log("Phase 2: Draw phase");
        m_activePlayer.DrawCards(1);

        ResolveStack();

        return CGTurnState.DEFAULT;
    }

    /** "Play" phase */
    CGTurnState Phase3()
    {
        Debug.Log("Phase 3: Play phase");

        List<int> playableCardIDs = m_activePlayer.m_hand.GetPlayableCardIDs();

        if (playableCardIDs.Count == 0)
        {
            TriggerEvent("SkipCard");
            ResolveStack();
            return CGTurnState.DEFAULT;
        }

        // Tell client to highlight playable cards
        CGC_WaitForPlayFromHand command = new CGC_WaitForPlayFromHand(playableCardIDs, m_activePlayerID);
        CGVisualManager.instance.AddCommand(command);

        // Wait for client to return chosen card
        return CGTurnState.WAIT_FOR_PLAY_CARD_FROM_HAND;
    }

    /** "Cast" phase */
    CGTurnState Phase4()
    {
        Debug.Log("Phase 4: Cast phase");
        if (m_activePlayerID == 0)   // @TODO: Update for multiplayer
        {
            return TellClientToCastSpell();
        }
        else
        {
            return CheckIfAIHasSpellsToCast();
        }
    }

    CGTurnState TellClientToCastSpell()
    {
        List<CGCardObject> spells = new List<CGCardObject>();
        foreach (CGCardObject s in m_activePlayer.GetSpells())
        {
            if (s.GetTimeRemaining() == 0)
            {
                spells.Add(s);
            }
        }
        if (spells.Count == 0)
        {
            ResolveStack();
            return CGTurnState.ALL_SPELLS_CAST;
        }
        else
        {
            // Send client list of spells
            List<CastableCard> castableCards = new List<CastableCard>();

            // Wait for client to pick oned
            foreach (CGCardObject spell in spells)
            {
                CastableCard card;
                card.cardID = spell.m_cardID;
                card.targets = GetAllValidTargetsForSpell(spell);
                castableCards.Add(card);
            }

            CGC_WaitForCastSelection command = new CGC_WaitForCastSelection(castableCards);
            CGVisualManager.instance.AddCommand(command);

            return CGTurnState.WAIT_FOR_CAST_ORDER;
        }
    }

    CGTurnState CheckIfAIHasSpellsToCast()
    {
        List<CGCardObject> spells = new List<CGCardObject>();
        foreach (CGCardObject s in m_activePlayer.GetSpells())
        {
            if (s.GetTimeRemaining() == 0)
            {
                spells.Add(s);
            }
        }
        if (spells.Count == 0)
        {
            ResolveStack();
            return CGTurnState.ALL_SPELLS_CAST;
        }
        else
        {
            return CGTurnState.WAIT_FOR_CAST_ORDER;
        }
    }

    CGTurnState TellAIToCastAllSpells()
    {
        List<CGCardObject> spells = new List<CGCardObject>();
        foreach (CGCardObject s in m_activePlayer.GetSpells())
        {
            if (s.GetTimeRemaining() == 0)
            {
                spells.Add(s);
            }
        }
        foreach(CGCardObject spell in spells)
        {
            if (spell.m_isSpell)
            {
                List<int> targets = GetAllValidTargetsForSpell(spell);
                if (targets.Count > 0)
                {
                    CastSpellFromChannel0(spell.m_cardID, targets[0]);
                }
                else
                {
                    CastSpellFromChannel0(spell.m_cardID, -1);
                }
            }
        }

        return CGTurnState.ALL_SPELLS_CAST;
    }
    
    /** "End" phase */
    CGTurnState Phase5()
    {
        Debug.Log("Phase 5: End phase");
        TriggerEvent("EndTurn");
        ResolveStack();
        return CGTurnState.DEFAULT;
    }
}
