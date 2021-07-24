using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CGChannelZone : CGZone {

    const float CARD_TITLE_HEIGHT = 60.0f;
    const float DEPTH_SPACING = 25.0f;

    protected override Sequence OnCardEnter(CardVisual card, float duration)
    {
        return AdjustCardPositionsSeq(duration);
    }

    protected override Sequence OnCardExit(CardVisual card, float duration)
    {
        return AdjustCardPositionsSeq(duration);
    }

    public Sequence AdjustCardPositionsSeq(float duration)
    {
        if(m_cards.Count == 0)
        {
            return null;
        }

        Sequence seq = DOTween.Sequence();

        int index = 0;
        foreach(CardVisual card in m_cards)
        {
            Vector3 pos = transform.position;
            pos.y += index * CARD_TITLE_HEIGHT;
            pos.z += index * DEPTH_SPACING;

            seq.Insert(0, MoveCardToPositionSeq(card, pos, duration));
            index++;
        }

        return seq;
    }
}
