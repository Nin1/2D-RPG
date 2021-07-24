using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDataPath : MonoBehaviour {

    public InputField input;

    public void SetCardDataPath(string path)
    {
        if (!path.EndsWith("/"))
        {
            path += "/";
        }
        if (!path.EndsWith("JSON/"))
        {
            path += "JSON/";
        }
        CardData.SetJSONPath(path);
    }

    public void SetCardDataPath()
    {
        string path = input.text;
        if (!path.EndsWith("/"))
        {
            path += "/";
        }
        if (!path.EndsWith("JSON/"))
        {
            path += "JSON/";
        }
        CardData.SetJSONPath(path);
    }
}
