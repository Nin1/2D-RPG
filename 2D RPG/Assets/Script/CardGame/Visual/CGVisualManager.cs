using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGVisualManager : MonoBehaviour {

    public static CGVisualManager instance;

    // @TODO: Contain these into one object for each player
    [Header("Player visuals")]
    public CGDeckZone m_playerDeck;
    public CGHandZone m_playerHand;
    public CGChannelZone[] m_playerChannels;
    public CGBasicZone m_playerTargettingZone;
    public CGBasicZone m_playerGraveyard;
    public CGLifeVisual m_playerLife;
    [Header("Opponent visuals")]
    public CGDeckZone m_opponentDeck;
    public CGHandZone m_opponentHand;
    public CGChannelZone[] m_opponentChannels;
    public CGBasicZone m_opponentTargettingZone;
    public CGBasicZone m_opponentGraveyard;
    public CGLifeVisual m_opponentLife;
    [Header("Misc")]
    public Transform m_focusZone;
    public Image m_phaseTransitionBG;
    public Text m_phaseTransitionText;
    public GameObject m_cardPrefab;
    public int m_playerID = 0;
     
    // Keep a list of all revealed cards in play
    public Dictionary<int, CardVisual> m_cards = new Dictionary<int, CardVisual>();

    ClientConnectionInterface m_connectionInterface;
    CGCommandRunner m_commandRunner;

	// Use this for initialization
	void Awake () {
        if(instance == null)
        {
            instance = this;
            m_commandRunner = new CGCommandRunner(this);
            //CardVisual.SetHotspots(m_playerDeck, m_opponentDeck, m_focusZone, m_playerHand, m_opponentHand, m_playerChannels, m_opponentChannels);
        }
        else
        {
            Destroy(gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        m_commandRunner.Update();
        if(!m_commandRunner.m_runningQueue)
        {
            m_playerHand.SetEnableHoverFocus(true);
        }
        else
        {
            m_playerHand.SetEnableHoverFocus(false);
        }
	}

    public string GetDeckFileName()
    {
        // @TODO: The heck is this doing in the connection interface
        return m_connectionInterface.m_deckFileName;
    }

    public void SetClientConnectionInterface(ClientConnectionInterface connection)
    {
        m_connectionInterface = connection;
    }

    public void TransmitStream(BKSystem.IO.BitStream stream)
    {
        m_connectionInterface.TransmitFromClient(stream);
    }

    public void SetPlayerID(int newID)
    {
        m_playerID = newID;
        Debug.Log("Set player ID to " + m_playerID);
    }

    /** Create a new card, set its attributes, and return its transform */
    public CardVisual CreateCard(CardData cardData, int cardID, bool faceUp)
    {
        if(m_cardPrefab == null)
        {
            Debug.LogError("Card Visual prefab not set");
        }

        // Create the new card
        GameObject newCard = GameObject.Instantiate(m_cardPrefab);

        if (faceUp)
        {
            newCard.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            newCard.transform.localEulerAngles = new Vector3(0, 180, 0);
        }

        CardVisual cv = newCard.GetComponent<CardVisual>();

        // cardData is null if the card is hidden (e.g. a card in the opponent's hand)
        if (cardData != null)
        {
            cv.SetCardData(cardData, cardID);
        }
        
        // Add the new card to the list by its ID
        m_cards.Add(cardID, cv);

        return cv;
    }

    /** Create a new blank card, face-down, and return its CardVisual */
    public CardVisual CreateHiddenCard()
    {
        if (m_cardPrefab == null)
        {
            Debug.LogError("Card Visual prefab not set");
        }

        // Create the new card
        GameObject newCard = GameObject.Instantiate(m_cardPrefab);
        
        // Turn it face-down
        newCard.transform.localEulerAngles = new Vector3(0, 180, 0);

        CardVisual cv = newCard.GetComponent<CardVisual>();
        
        return cv;
    }

    /** Define the data of a blank card */
    public CardVisual DefineCard(CardVisual card, CardData data, int cardID)
    {
        card.SetCardData(data, cardID);
        m_cards.Add(cardID, card);

        return card;
    }

    public CardVisual GetCard(int cardID)
    {
        if(m_cards.ContainsKey(cardID))
        {
            return m_cards[cardID];
        }
        else
        {
            return CreateCard(null, cardID, false);
        }
    }
    
    public void AddCommand(CGCommand command)
    {
        m_commandRunner.AddToQueue(command);
    }

    public void MakeClickable(List<int> cardIDs, CardVisual.OnCardClick callback)
    {
        foreach (int cID in cardIDs)
        {
            m_cards[cID].SetClickable(true);
            m_cards[cID].SetOnClickFunction(callback);
        }
    }

    public void MakeAllUnclickable()
    {
        foreach (KeyValuePair<int, CardVisual> card in m_cards)
        {
            card.Value.SetClickable(false);
        }
    }

    public void ClickCardWithID(int cardID)
    {
        m_cards[cardID].SimulateClick();
    }

    /** Zone-Getters */

    public CGHandZone GetPlayerHand()
    {
        return m_playerHand;
    }

    public CGDeckZone GetPlayerDeck()
    {
        return m_playerDeck;
    }

    public CGChannelZone GetPlayerChannel(int channel)
    {
        return m_playerChannels[channel];
    }

    public CGBasicZone GetPlayerTargettingZone()
    {
        return m_playerTargettingZone;
    }

    public CGBasicZone GetPlayerGraveyard()
    {
        return m_playerGraveyard;
    }

    public CGLifeVisual GetPlayerLife()
    {
        return m_playerLife;
    }

    public CGHandZone GetOpponentHand()
    {
        return m_opponentHand;
    }

    public CGDeckZone GetOpponentDeck()
    {
        return m_opponentDeck;
    }

    public CGChannelZone GetOpponentChannel(int channel)
    {
        return m_opponentChannels[channel];
    }

    public CGBasicZone GetOpponentTargettingZone()
    {
        return m_opponentTargettingZone;
    }

    public CGBasicZone GetOpponentGraveyard()
    {
        return m_opponentGraveyard;
    }

    public CGLifeVisual GetOpponentLife()
    {
        return m_opponentLife;
    }

    public Image GetPhaseTransitionBackground()
    {
        return m_phaseTransitionBG;
    }

    public Text GetPhaseTransitionText()
    {
        return m_phaseTransitionText;
    }
}
