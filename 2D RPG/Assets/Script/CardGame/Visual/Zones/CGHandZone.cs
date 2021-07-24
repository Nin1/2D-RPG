using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CGHandZone : CGZone {

    public Transform m_mouseOverPos;

    bool m_enableHoverFocus = false;

    protected override Sequence OnCardEnter(CardVisual card, float duration)
    {
        return AdjustCardPositionsSeq(duration);
    }

    protected override Sequence OnCardExit(CardVisual card, float duration)
    {
        return AdjustCardPositionsSeq(duration);
    }

    private void Update()
    {
        if (m_enableHoverFocus)
        {
            foreach (CardVisual card in m_cards)
            {
                // Check if the mouse is over each card and set the hover appropriately
                if (card.IsMouseOver())
                {
                    if (!card.m_isHoverFocused)
                    {
                        Vector3 cardPos = card.transform.position;
                        cardPos.y = transform.position.y + m_mouseOverPos.transform.localPosition.y;
                        //cardPos.z = cardPos.z + m_mouseOverPos.transform.localPosition.z;
                        MoveCardToPosition(card, cardPos, 0.2f);
                        card.m_isHoverFocused = true;
                    }
                }
                else if (card.m_isHoverFocused)
                {
                    Vector3 cardPos = card.transform.position;
                    cardPos.y = transform.position.y;
                    MoveCardToPosition(card, cardPos, 0.2f);
                    card.m_isHoverFocused = false;
                }
            }
        }
    }

    public void SetEnableHoverFocus(bool enable)
    {
        m_enableHoverFocus = enable;
        if(!enable)
        {
            //AdjustCardPositions();
        }
    }

    /** Return the world-space position of a new card if it were to enter the hand */
    public Vector3 GetPositionForNewCard()
    {
        int newHandWidth = (m_cards.Count + 1) * 400;
        float xPos = (newHandWidth / 2) - (CARD_WIDTH / 2);

        return transform.position + new Vector3(xPos, 0, 0);
    }

    /** Move all cards currently in the hand to accommodate for a new card */
    public void MakeSpaceForNewCard(float duration)
    {
        // Calculate new position for each card

        int newHandWidth = (m_cards.Count + 1) * 400;

        int i = 0;
        foreach(CardVisual cv in m_cards)
        {
            float xPos = (-newHandWidth / 2) + (CARD_WIDTH / 2) + (CARD_WIDTH * i);
            // Run DOTween for each card
            cv.MoveToPosition(transform.position + new Vector3(xPos, 0, 0), duration);
            i++;
        }
    }

    public Sequence AdjustCardPositionsSeq(float duration)
    {
        if (m_cards.Count == 0)
        {
            return null;
        }

        int newHandWidth = (m_cards.Count) * 400;
        Sequence seq = DOTween.Sequence();

        int i = 0;
        foreach (CardVisual cv in m_cards)
        {
            float xPos = (-newHandWidth / 2) + (CARD_WIDTH / 2) + (CARD_WIDTH * i);
            // Generate tween sequence for each card
            seq.Insert(0, MoveCardToPositionSeq(cv, transform.position + new Vector3(xPos, 0, 0), duration));
            i++;
        }

        return seq;
    }

    void AdjustCardPositions()
    {
        int newHandWidth = (m_cards.Count) * 400;

        int i = 0;
        foreach (CardVisual cv in m_cards)
        {
            float xPos = (-newHandWidth / 2) + (CARD_WIDTH / 2) + (CARD_WIDTH * i);
            // Generate tween sequence for each card
            cv.transform.position = transform.position + new Vector3(xPos, 0, 0);
            i++;
        }
    }
}
