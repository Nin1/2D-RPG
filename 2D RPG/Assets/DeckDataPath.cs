using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckDataPath : MonoBehaviour
{
    public InputField input;

    public void SetDeckFolderPath(string path)
    {
        if (!path.EndsWith("/"))
        {
            path += "/";
        }
        PackedDeck.SetDeckFolder(path);
    }

    public void SetDeckFolderPath()
    {
        string path = input.text;
        if (!path.EndsWith("/"))
        {
            path += "/";
        }
        PackedDeck.SetDeckFolder(path);
    }
}
