﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CGBasicZone : CGZone
{
    protected override Sequence OnCardEnter(CardVisual card, float duration)
    {
        return MoveCardToPositionSeq(card, transform.position, duration);
    }

    protected override Sequence OnCardExit(CardVisual card, float duration)
    {
        return null;
    }
}