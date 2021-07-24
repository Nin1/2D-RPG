using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecklistEntry : MonoBehaviour {

    [SerializeField]
    Text m_cardName;
    [SerializeField]
    UI_ChannelCost m_cost;

    private CardData m_cardData;
    private Transform m_tooltip;

    public delegate void OnClick(DecklistEntry entry);
    event OnClick m_onClickEvent;

    public void SetAsCard(CardData card)
    {
        m_cardName.text = card.cardName;
        m_cost.SetCost(card.channelCost);
    }

    public CardData GetCardData()
    {
        return m_cardData;
    }
    	
    public void SetOnClickEvent(OnClick onClick)
    {
        m_onClickEvent += onClick;
    }

    public void RemoveOnClickEvent(OnClick onClick)
    {
        m_onClickEvent -= onClick;
    }

    private void OnMouseEnter()
    {
        // ShowCardTooltip()
    }

    private void OnMouseExit()
    {
        // HideCardTooltip()
    }

    private void OnMouseDown()
    {
        if (m_onClickEvent != null)
        {
            m_onClickEvent(this);
        }
    }
}
