using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class DeckEditor : EditorWindow
{
    Vector2 m_deckScrollPos;
    Vector2 m_cardsScrollPos;
    List<CardData> m_cards = new List<CardData>();
    List<CardData> m_deck = new List<CardData>();
    CardData m_currentCard;
    int m_selectedIndex = 0;

    [MenuItem("Window/Card Editor")]
    public static void ShowWindow()
    {
        CardEditor cardEditor = (CardEditor)EditorWindow.GetWindow<CardEditor>("Card Editor", typeof(CardEditorList));
        cardEditor.LoadListOfCards();
        cardEditor.Show();
    }

    private void OnGUI()
    {
        //CreateCardDataEditor();
        EditorGUILayout.BeginHorizontal();
        DeckList();
        ListOfCards();
        EditorGUILayout.EndHorizontal();
    }

    void DeckList()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Deck list", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                AddSelectedCardToDeck();
            }
        }
        EditorGUILayout.EndHorizontal();

        int cardCount = m_deck.Count;
        for(int i = 0; i < cardCount; i++)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(m_deck[i].cardName, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    m_deck.RemoveAt(i);
                    cardCount--;
                }
            }
            EditorGUILayout.EndHorizontal();

            i++;
        }

    }

    void AddSelectedCardToDeck()
    {
        m_deck.Add(m_currentCard);
    }

    void ListOfCards()
    {
        EditorGUILayout.BeginVertical();

        // Set up the list style
        Color color_default = GUI.backgroundColor;
        Color color_selected = Color.gray;
        GUIStyle itemStyle = new GUIStyle(GUI.skin.button);
        itemStyle.alignment = TextAnchor.MiddleLeft; //align text to the left
        itemStyle.active.background = itemStyle.normal.background;  //gets rid of button click background style.
        itemStyle.margin = new RectOffset(0, 0, 0, 0); //removes the space between items (previously there was a small gap between GUI which made it harder to select a desired item)

        // Build the list
        m_cardsScrollPos = EditorGUILayout.BeginScrollView(m_cardsScrollPos, GUILayout.Width(200), GUILayout.Height(250));
        int index = 0;
        foreach (CardData card in m_cards)
        {
            GUI.backgroundColor = (m_selectedIndex == index) ? color_selected : Color.clear;
            if (GUILayout.Button(card.cardName, itemStyle))
            {
                SelectNewCard(index);
            }
            index++;
        }
        EditorGUILayout.EndScrollView();
        // Reset the style
        GUI.backgroundColor = color_default;

        EditorGUILayout.EndVertical();
    }

    public void LoadListOfCards()
    {
        // Get list of all cards in cards directory
        string[] fileList = Directory.GetFiles(CardData.cardJSONPath, "*.json");

        // Load each card and add it to the list
        foreach (string path in fileList)
        {
            CardData card = CardData.LoadCardData(path);
            Debug.Log("Loaded card " + card.cardName);
            m_cards.Add(card);
        }

        SelectNewCard(0);
        // EditorGUILayout.Foldout for attributes
    }

    void SelectNewCard(int cardIndex)
    {
        // Update current card
        m_selectedIndex = cardIndex;
        m_currentCard = m_cards[m_selectedIndex];
        GUI.FocusControl(null);
    }
}
