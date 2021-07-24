using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CGDeckZone : CGZone {
    
    protected override Sequence OnCardEnter(CardVisual card, float duration)
    {
        Debug.Log("Placing card on deck");
        return PlaceCardOnTopSeq(card, duration);
    }

    protected override Sequence OnCardExit(CardVisual card, float duration)
    {
        return null;
    }

    public Sequence PlaceCardOnTopSeq(CardVisual card, float duration)
    {
        return MoveCardToPositionSeq(card, transform.position, duration);
    }
}
