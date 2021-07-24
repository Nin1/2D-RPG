using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// @TODO: Move cards to canvas and render them in screen-space not world-space
public class CGSelectionZone : CGZone {

    public Transform m_menuCenter;
    public Transform m_castBox;
    public Transform m_targetBox;

    [SerializeField]
    public Text m_menuTitle;
    [SerializeField]
    public Text m_menuDesc;

    const float CARD_PADDING = 30.0f;
    const float VERTICAL_CARD_SPACING = CARD_HEIGHT + CARD_PADDING;
    const float HORIZONTAL_CARD_SPACING = CARD_WIDTH + CARD_PADDING;
    
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
        if (m_cards.Count == 0)
        {
            return null;
        }

        Sequence seq = DOTween.Sequence();

        // Calculate the number of rows
        int numRows = (int) Mathf.Sqrt(m_cards.Count);
        int cardsPerRow = Mathf.RoundToInt(m_cards.Count / numRows);
        int maxCardsInCurrentRow = cardsPerRow;

        // Calculate y-position of first row
        float offsetFactor = (numRows - 1) / 2.0f;
        float currentRowYPos = offsetFactor * VERTICAL_CARD_SPACING;

        // Set up counting variables
        int currentRow = 0;
        int cardsInCurrentRow = 0;
        int cardsPlaced = 0;

        // Place cards one-by-one
        foreach (CardVisual card in m_cards)
        {
            // Calculate x-position of this card
            float rowWidth = maxCardsInCurrentRow * HORIZONTAL_CARD_SPACING;
            float xPos = (-rowWidth / 2.0f) + (HORIZONTAL_CARD_SPACING / 2.0f) + (HORIZONTAL_CARD_SPACING * cardsInCurrentRow);

            // Create the final position of the card
            Vector3 finalPos = m_menuCenter.position;
            finalPos.y += currentRowYPos;
            finalPos.x += xPos;

            // Create the tween sequence
            seq.Insert(0, MoveCardToPositionSeq(card, finalPos, duration));

            // Increment the cards-in-row count
            cardsPlaced++;
            cardsInCurrentRow++;
            if(cardsInCurrentRow == maxCardsInCurrentRow)
            {
                // Reset the cards-in-row count
                cardsInCurrentRow = 0;
                // Move to the next row
                currentRow++;
                // Calculate whether the next row has less cards (e.g. a row of 3, then a row of 2 for 5 cards)
                maxCardsInCurrentRow = Mathf.Min(cardsPerRow, (m_cards.Count - cardsPlaced));
                currentRowYPos -= VERTICAL_CARD_SPACING;
            }
        }

        return seq;
    }

    /** Close the menu, returning all un-selected cards to the given zone */
    public void Close(CGZone previousZone)
    {
        List<CardVisual> unselectedCards = new List<CardVisual>();

        foreach(CardVisual card in m_cards)
        {
            unselectedCards.Add(card);
        }

        foreach(CardVisual unselectedCard in unselectedCards)
        {
            previousZone.MoveCardToZone(unselectedCard);
        }

        // Close the menu
        Destroy(gameObject);
    }
}
