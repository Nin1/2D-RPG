using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_Decklist : MonoBehaviour {

    [SerializeField]
    Transform m_entryPrefab;

    List<DecklistEntry> m_decklist = new List<DecklistEntry>();
    
    public void LoadDecklist(string path)
    {

    }

    public void AddCardToList(CardData card)
    {
        // Create the entry graphic
        RectTransform newEntry = (RectTransform)Instantiate(m_entryPrefab, transform);
        DecklistEntry entryData = newEntry.GetComponent<DecklistEntry>();
        entryData.SetAsCard(card);
        entryData.SetOnClickEvent(EntryClickCallback);
        float yPos = -45 * (m_decklist.Count - 1) - 25;
        newEntry.anchoredPosition = new Vector3(0, yPos, 0);

        m_decklist.Add(entryData);
    }

    void EntryClickCallback(DecklistEntry entry)
    {
        // When an entry in the decklist is clicked, remove it
        m_decklist.Remove(entry);
        entry.RemoveOnClickEvent(EntryClickCallback);
        Destroy(entry.gameObject);
    }

    void ReorderList()
    {
        // Sort m_decklist alphabetically by name
        // For each entry in m_decklist
        //      calculate new y position
        //      move to new y position 
    }

    // @TODO:
    // Create "DB_Collection" to manage the collection display
    // DB_Collection can have click callback similar to DB_Decklist
    // DB_Collection has reference to DB_Decklist so that it can add cards
}
