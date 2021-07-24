using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

class CardEditor : EditorWindow {

    Vector2 m_scrollPos;
    Vector2 m_attribScrollPos;
    List<CardData> m_cards = new List<CardData>();
    CardData m_currentCard;
    int m_selectedIndex = 0;
    int m_highestID = -1;

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
        CardDataEditor();
        ListOfCards();
        EditorGUILayout.EndHorizontal();
    }

    void CardDataEditor()
    {
        if (m_currentCard != null)
        {
            EditorGUILayout.BeginVertical();
            m_currentCard.cardName = EditorGUILayout.TextField("Card name:", m_currentCard.cardName);
            m_currentCard.channelCost = int.Parse(EditorGUILayout.TextField("Card cost:", m_currentCard.channelCost.ToString()));

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Card text:", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                m_currentCard.rulesText = EditorGUILayout.TextArea(m_currentCard.rulesText, GUILayout.Width(300), GUILayout.Height(50));
            }
            EditorGUILayout.EndHorizontal();

            m_currentCard.cardScriptName = EditorGUILayout.TextField("Script:", m_currentCard.cardScriptName);
            m_currentCard.hasTargets = EditorGUILayout.Toggle("Has target?", m_currentCard.hasTargets);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Card Data ID:", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(m_currentCard.dataID.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            EditorGUILayout.EndHorizontal();

            ShowCardAttributes();

            EditorGUILayout.EndVertical();
            
            m_cards[m_selectedIndex] = m_currentCard;
        }
    }

    void ShowCardAttributes()
    {
        // Display Card Attributes label with an "Add" button
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Card Attributes:");
            // Add new attribute button
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                CardAttribute newAttrib = new CardAttribute();
                newAttrib.name = "NewAttribute";
                newAttrib.value = 0;
                m_currentCard.cardAttributes.Add(newAttrib);
            }

        }
        EditorGUILayout.EndHorizontal();

        // Display each attribute
        EditorGUI.indentLevel++;
        m_attribScrollPos = EditorGUILayout.BeginScrollView(m_attribScrollPos, GUILayout.Height(170));
        int attribCount = m_currentCard.cardAttributes.Count;
        for(int i = 0; i < attribCount; i++)
        {
            CardAttribute attrib = m_currentCard.cardAttributes[i];

            // Display attribute number with a "Remove" button
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField((i + 1).ToString());
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    // Remove the attribute and continue to the next one
                    // Removing attribute within for loop is a bit dodgy so be careful here
                    m_currentCard.cardAttributes.RemoveAt(i);
                    attribCount--;  // Decrement attribute count, do not increment i
                    continue;
                }
            }
            EditorGUILayout.EndHorizontal();

            // Show the attribute data
            EditorGUI.indentLevel++;
            attrib.name = EditorGUILayout.TextField("Name:", attrib.name);
            attrib.value = int.Parse(EditorGUILayout.TextField("Value:", attrib.value.ToString()));
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndScrollView();
        EditorGUI.indentLevel--;
    }

    void SelectNewCard(int cardIndex)
    {
        // Update current card
        m_selectedIndex = cardIndex;
        m_currentCard = m_cards[m_selectedIndex];
        GUI.FocusControl(null);
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
        m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.Width(200), GUILayout.Height(250));
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

        Rect r = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add New Card", GUILayout.Width(100)))
        {
            CreateNewCardData();
        }
        if (GUILayout.Button("Remove Card", GUILayout.Width(100)))
        {
            RemoveSelectedCard();
        }
        EditorGUILayout.EndHorizontal();
        
        if(GUILayout.Button("Export Cards"))
        {
            ExportCards();
        }

        EditorGUILayout.EndVertical();
    }

    void CreateNewCardData()
    {
        CardData newCard = new CardData();
        newCard.cardName = "New Card";
        newCard.channelCost = 1;
        newCard.cardScriptName = "Script_NewCard";
        newCard.hasTargets = false;
        newCard.rulesText = "Do new card things.";
        m_highestID++;
        newCard.dataID = m_highestID;

        // Add and select the new card
        m_cards.Add(newCard);
        SelectNewCard(m_cards.Count - 1);
    }

    void RemoveSelectedCard()
    {
        m_cards.RemoveAt(m_selectedIndex);
        // Select next card if one exists
        m_selectedIndex = Mathf.Max(0, m_selectedIndex - 1);
        if (m_cards.Count > m_selectedIndex)
        {
            SelectNewCard(m_selectedIndex);
        }
        else
        {
            m_currentCard = null;
        }
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

        GiveCardsUniqueIDs();

        SelectNewCard(0);
        // EditorGUILayout.Foldout for attributes
    }

    void GiveCardsUniqueIDs()
    {
        // Find the highest ID already used
        foreach (CardData card in m_cards)
        {
            if (card.dataID > m_highestID)
            {
                m_highestID = card.dataID;
            }
        }

        // Give any cards without IDs an ID
        for(int i = 0; i < m_cards.Count; i++)
        {
            CardData card = m_cards[i];
            if(card.dataID == -1)
            {
                m_highestID++;
                card.dataID = m_highestID;
            }
            m_cards[i] = card;
        }
    }

    void ExportCards()
    {
        foreach (CardData card in m_cards)
        {
            // create filename
            if(card.fileName == null)
            {
                card.fileName = card.dataID.ToString() + "_" + card.cardName.Replace(" ", string.Empty) + ".json";
            }

            // create json data
            CardDataJSONRoot data = new CardDataJSONRoot();
            data.cardData = card;
            string output = JsonConvert.SerializeObject(data, Formatting.Indented);

            // output to file
            StreamWriter outFile = new StreamWriter(CardData.cardJSONPath + card.fileName);
            outFile.Write(output);
            outFile.Close();
        }
    }
}
