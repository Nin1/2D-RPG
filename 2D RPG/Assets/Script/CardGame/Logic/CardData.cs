using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

// JsonConvert.DeserializeObject requires a root object to create
public class CardDataJSONRoot
{
    public CardData cardData { get; set; }
}

public class CardData
{
    public static string cardJSONPath = "D:/Projects/Unity/2D RPG/CardDB/JSON/";
    public static void SetJSONPath(string path)
    {
        if(!path.EndsWith("/"))
        {
            path += "/";
        }
        cardJSONPath = path;
    }

    public string fileName { get; set; }
    public string cardName { get; set; }
    public string rulesText { get; set; }
    public bool hasTargets { get; set; }
    public string cardScriptName { get; set; }
    public int dataID = -1;
    // Variable
    public int channelCost { get; set; }
    public List<CardAttribute> cardAttributes = new List<CardAttribute>();
    

    /** @param dbName = the file name of the card's json file */
    public static CardData LoadCardData(string path)
    {
        if (!path.Contains(".json"))
        {
            Debug.LogWarning("CardData attempted to load without a .json extension!");
            path = path + ".json";
        }
        Debug.Log("Loading card: " + path);
        StreamReader sr = new StreamReader(path);

        string contents = sr.ReadToEnd();

        CardDataJSONRoot newData = JsonConvert.DeserializeObject<CardDataJSONRoot>(contents);
        sr.Close();
        return newData.cardData;
    }

    static void LoadJSON(string path)
    {
    }

    public void SetAttribute(string name, int value)
    {
        // If the attribute already exists, set it
        foreach(CardAttribute attrib in cardAttributes)
        {
            if(attrib.name == name)
            {
                attrib.value = value;
                return;
            }
        }

        // If the attribute doesn't exist, create it
        CardAttribute attribute = new CardAttribute();
        attribute.name = name;
        attribute.value = value;

        cardAttributes.Add(attribute);
    }
}
