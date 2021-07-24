using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour {

    [SerializeField]
    Text m_text;
    [SerializeField]
    Image m_portrait;
    [SerializeField]
    GameObject m_nameText;
    [SerializeField]
    ChoiceList m_choiceList;

    private string m_string;
    private string m_visibleString;
    private int m_count = 0;
    private bool m_isComplete = false;

    private float m_secondsPerChar = 0.0f;
    private float m_timeSinceLastChar = 0.0f;
    List<string> m_choices = new List<string>();
    int m_choiceIndex = -1;
    
    public void StartDialogue(string dialogue, string name = null, string portraitName = null, float secondsPerChar = 0.05f)
    {
        m_timeSinceLastChar = 0.0f;
        m_secondsPerChar = secondsPerChar;
        m_text.text = "";
        m_isComplete = false;
        m_count = 0;
        m_string = dialogue;
        m_visibleString = "";
        m_choices.Clear();
        m_choiceIndex = -1;

        SetPortrait(portraitName);
        SetName(name);
        m_choiceList.gameObject.SetActive(false);
    }

    public void StartChoice(string dialogue, string[] choices, string name = null, string portraitName = null, float secondsPerChar = 0.05f)
    {
        StartDialogue(dialogue, name, portraitName, secondsPerChar);
        SetChoices(choices);
    }

    void SetName(string name)
    {
        m_nameText.SetActive(name != null);
        m_nameText.GetComponentInChildren<Text>().text = name;
    }

    void SetPortrait(string portraitID)
    {
        m_portrait.gameObject.SetActive(portraitID != null);
        // TODO: Show a portrait
    }

    void SetChoices(string[] choices)
    {
        // Prepare a list of strings for being sent to the ChoiceList
        m_choices.Clear();
        foreach (string s in choices)
        {
            m_choices.Add(s);
        }
    }

	// Update is called once per frame
	void Update () {
		if(m_visibleString != m_string)
        {
            // If the string has not finished displaying yet, display the list
            if(m_count > 0 && InputManager.GetButtonPressed(InputButton.ACTION))
            {
                m_visibleString = m_string;
            }
            else
            {
                m_timeSinceLastChar += Time.deltaTime;
                if (m_timeSinceLastChar >= m_secondsPerChar)
                {
                    m_timeSinceLastChar -= m_secondsPerChar;
                    m_visibleString += m_string[m_count];
                    ++m_count;
                }
            }
            m_text.text = m_visibleString;

            // If we are now showing the whole string, check whether we need to show any choices
            if(m_visibleString == m_string)
            {
                ShowChoices();
            }
        }
        else if(InputManager.GetButtonPressed(InputButton.ACTION))
        {
            m_isComplete = true;

            // Get the choice that was selected if there was one
            if(m_choices.Count > 0)
            {
                m_choiceIndex = m_choiceList.GetSelectedChoice();
            }
        }
	}

    // Initialise the choice list if there are any choices
    void ShowChoices()
    {
        if(m_choices.Count > 0)
        {
            m_choiceList.InitialiseChoices(m_choices);
            m_choiceList.gameObject.SetActive(true);
        }
    }

    public int GetChoiceIndex()
    {
        return m_choiceIndex;
    }

    public bool IsComplete()
    {
        return m_isComplete;
    }
}
