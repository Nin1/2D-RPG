using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum CardLocation
{
    NONE,
    PLAYER_DECK,
    OPPONENT_DECK,
    PLAYER_HAND,
    OPPONENT_HAND,
    PLAYER_CHANNEL,
    OPPONENT_CHANNEL
}

public enum CardFace
{
    DEFAULT,
    FACE_UP,
    FACE_DOWN
}

public class CardVisual : MonoBehaviour {

    public Image m_cardImage;
    public Text m_cardName;
    public Text m_cardType;
    public Text m_cardText;
    public Text m_channelCost;
    public Text m_cooldownCost;
    public Image m_glow;

    public delegate void OnCardClick(CardVisual card);
    event OnCardClick onCardClick;

    public int m_cardID { get; private set; }
    public CGZone m_currentZone { get; set; }
    public CardLocation m_location = CardLocation.NONE;
    public bool m_hoverFocusEnabled { get; private set; }
    public bool m_isHoverFocused = false;
    bool m_clickable = false;
    bool m_mouseOver = false;
    Sequence m_sequence;

    static Camera m_camera;
    static CGDeckZone m_playerDeckPos;
    static CGDeckZone m_opponentDeckPos;
    static Transform m_focusZonePos;
    static CGHandZone m_playerHand;
    static CGHandZone m_opponentHand;
    static CGChannelZone[] m_playerChannels;
    static CGChannelZone[] m_opponentChannels;

    /** SETUP */

    void Awake()
    {
        if(m_camera == null)
        {
            m_camera = Camera.main;
        }

        m_cardID = -1;
    }

    /*
    public static void SetHotspots(
        Transform playerDeck,
        Transform oppDeck,
        Transform focusZone,
        CGHandZone playerHand,
        CGHandZone oppHand,
        Transform[] playerChannels,
        Transform[] opponentChannels)
    {
        m_playerDeckPos = playerDeck;
        m_opponentDeckPos = oppDeck;
        m_focusZonePos = focusZone;
        m_playerHand = playerHand;
        m_opponentHand = oppHand;
        m_playerChannels = playerChannels;
        m_opponentChannels = opponentChannels;
    }
    */

    public void SetCardData(CardData card, int cardID)
    {
        m_cardName.text = card.cardName;
        m_cardType.text = "Spell";
        m_cardText.text = card.rulesText;
        m_channelCost.text = card.channelCost.ToString();
        m_cooldownCost.text = "0";
        m_cardID = cardID;
        m_cooldownCost.text = cardID.ToString();
    }

    string CardTypeToString(CardType cardType)
    {
        switch(cardType)
        {
            case CardType.FOCUS:
                return "Focus";
            case CardType.SPELL:
                return "Spell";
            case CardType.PERMANENT:
                return "Permanent";
            default:
                return "Unknown";
        }
    }


    /** INTERACTION */

    public void SetClickable(bool clickable)
    {
        m_glow.gameObject.SetActive(clickable);
        m_clickable = clickable;
    }

    /** Remove all previously set callback functions for when the card is clicked, and set a new one */
    public void SetOnClickFunction(OnCardClick callback)
    {
        onCardClick = callback;
    }

    /** Adds a new callback function to the list of functions called when the card is clicked */
    public void AppendOnClickFunction(OnCardClick callback)
    {
        onCardClick += callback;
    }

    void Update()
    {
        if (m_clickable && m_mouseOver && Input.GetButtonDown("Action"))
        {
            // Call OnClick delegates
            if (onCardClick != null)
            {
                onCardClick(this);
            }
        }
    }

    /** Simulates this card being clicked */
    public void SimulateClick()
    {
        if (m_clickable)
        {
            // Call OnClick delegates
            if (onCardClick != null)
            {
                onCardClick(this);
            }
        }
    }

    private void OnMouseOver()
    {
        m_mouseOver = true;
        m_glow.color = Color.green;
    }

    private void OnMouseExit()
    {
        m_mouseOver = false;
        m_glow.color = Color.white;
    }

    public bool IsMouseOver()
    {
        return m_mouseOver;
    }

    /** APPEARANCE/MOVEMENT */

    public Sequence SetFaceDirectionSeq(bool faceUp, float duration)
    {
        if (faceUp)
        {
            return DOTween.Sequence()
                .Append(transform.DORotate(new Vector3(0, 0, 0), duration)
                    .SetEase(Ease.InOutSine));
        }
        else
        {
            return DOTween.Sequence()
                .Append(transform.DORotate(new Vector3(0, 180, 0), duration)
                    .SetEase(Ease.InOutSine));
        }
    }

    public void SetFaceDirection(bool faceUp)
    {
        if (faceUp)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
    }

    public void MoveToPosition(Vector3 position, float duration)
    {
        m_sequence = DOTween.Sequence()
            .Append(transform.DOMove(position, duration)
                .SetEase(Ease.InOutSine));

        m_sequence.Play();
    }

    public Sequence MoveToPositionSeq(Vector3 position, float duration)
    {
        return DOTween.Sequence()
            .Append(transform.DOMove(position, duration)
                .SetEase(Ease.InOutSine));
    }

    /** Tell CGHandVisual that this card can be focused on mouse-over */
    public void EnableHoverFocus()
    {
        m_hoverFocusEnabled = true;
    }

}
