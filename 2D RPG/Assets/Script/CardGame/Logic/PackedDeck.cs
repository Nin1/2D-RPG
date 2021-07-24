using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

// JsonConvert.DeserializeObject requires a root object to create
public class PackedDeckJSONRoot
{
    public PackedDeck packedDeck { get; set; }
}

// Packed Deck
// Minimal information about a deck
public class PackedDeck
{
    public static string deckPath = "D:/Projects/Unity/2D RPG/Decks/";
    public static void SetDeckFolder(string path)
    {
        if (!path.EndsWith("/"))
        {
            path += "/";
        }
        deckPath = path;
    }

    public string deckName { get; set; }
    public List<int> cardIDs = new List<int>();

    public PackedDeck()
    {
    }

    public void LoadFromJSON(string path)
    {
        if (!path.Contains(".json"))
        {
            Debug.LogWarning("CardData attempted to load without a .json extension!");
            path = path + ".json";
        }
        Debug.Log("Loading deck: " + path);
        StreamReader sr = new StreamReader(path);

        string contents = sr.ReadToEnd();

        PackedDeckJSONRoot newData = JsonConvert.DeserializeObject<PackedDeckJSONRoot>(contents);
        sr.Close();

        deckName = newData.packedDeck.deckName;
        cardIDs = newData.packedDeck.cardIDs;
    }


}
