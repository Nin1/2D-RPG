using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class CGZone : MonoBehaviour
{
    public const float DEFAULT_DURATION = 0.4f;
    public const float CARD_WIDTH = 400.0f;
    public const float CARD_HEIGHT = 600.0f;
    protected List<CardVisual> m_cards = new List<CardVisual>();

    public Sequence MoveCardToZone(CardVisual card, float duration = DEFAULT_DURATION)
    {
        Sequence seq = DOTween.Sequence();
        Sequence onExit = null;
        Sequence onEnter = null;
        
        if (card.m_currentZone != null)
        {
            // Remove the card from its last zone's list
            card.m_currentZone.MoveCardFromZone(card);
            // Add the card's last zone's "leaving" sequence
            onExit = card.m_currentZone.OnCardExit(card, duration);
        }

        card.m_currentZone = this;
        // Add the card to this zone's list
        m_cards.Add(card);

        // Add the zone's "OnMoveCardToZone" sequence
        onEnter = OnCardEnter(card, duration);

        // If there are no enter/exit animations, return null
        if (onEnter == null && onExit == null)
        {
            return DOTween.Sequence().Append(transform.DOScaleZ(1.0f, 0.0f));   // dummy tween (assuming that scale Z isn't used, DANGEROUS). Note that an empty sequence (DOTween.Sequence()) breaks any parent sequence.
        }

        if (onExit != null)
        {
            seq.Insert(0, onExit);
        }
        if (onEnter != null)
        {
            seq.Insert(0, onEnter);
        }

        return seq;
    }

    /** Called when a card moves from this zone to remove it from the cards list */
    private void MoveCardFromZone(CardVisual card)
    {
        m_cards.Remove(card);
    }

    public CardVisual GetCardWithID(int id)
    {
        foreach(CardVisual card in m_cards)
        {
            if(card.m_cardID == id)
            {
                return card;
            }
        }
        return null;
    }

    /** Returns the first card in the zone - Used for removing an 'unknown' card from the opponent's hand */
    public CardVisual GetFirstCard()
    {
        if (m_cards.Count > 0)
        {
            return m_cards[0];
        }
        return null;
    }

    /** Called when a card enters this zone (After it is added to the m_cards list) */
    protected abstract Sequence OnCardEnter(CardVisual card, float duration = DEFAULT_DURATION);

    /** Called when a card leaves this zone (After it is removed from the m_cards list) */
    protected abstract Sequence OnCardExit(CardVisual card, float duration = DEFAULT_DURATION);

    protected Sequence MoveCardToPositionSeq(CardVisual card, Vector3 position, float duration = DEFAULT_DURATION)
    {
        return DOTween.Sequence()
            .Append(card.transform.DOMove(position, duration)
                .SetEase(Ease.InOutSine));
    }

    protected void MoveCardToPosition(CardVisual card, Vector3 position, float duration = DEFAULT_DURATION)
    {
        Sequence seq = DOTween.Sequence()
            .Append(card.transform.DOMove(position, duration)
                .SetEase(Ease.InOutSine));

        seq.Play();
    }
}
