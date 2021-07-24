using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceList : MonoBehaviour {

    [SerializeField]
    Transform m_arrow;

    static readonly float TEXT_BORDER = 5.0f;
    List<string> m_choices = new List<string>();
    int m_currentIndex = 0;

    void Start()
    {
    }

    void Update()
    {
        if (InputManager.GetDirectionPressed(InputDirection.MAIN_UP))
        {
            SetArrow(m_currentIndex - 1);
        }
        else if (InputManager.GetDirectionPressed(InputDirection.MAIN_DOWN))
        {
            SetArrow(m_currentIndex + 1);
        }
    }

    public int GetSelectedChoice()
    {
        if(m_choices.Count == 0)
        {
            return -1;
        }
        return m_currentIndex;
    }

    public void InitialiseChoices(List<string> choices)
    {
        if(choices.Count == 0)
        {
            Debug.LogError("Dialogue choice given with no options.");
        }
        m_choices = choices;

        // Set position of text
        SetTextPosition();

        // Set position of background box
        SetBackgroundPosition();

        // Set position of arrow to option 0
        SetArrow(0);
    }

    void SetBackgroundPosition()
    {
        Text text = GetComponentInChildren<Text>();
        var rtransform = GetComponent<RectTransform>();

        // Get width/height of textbox
        float width = text.preferredWidth + (2 * TEXT_BORDER);
        float height = text.preferredHeight + (2 * TEXT_BORDER);

        // Set width to fit longest choice
        // Set height to fit MIN(number of choices, max displayable choices with up/down scrolling)
        float localx = -width / 2;
        float localy = height / 2;

        rtransform.anchoredPosition = new Vector2(localx, localy);
        rtransform.sizeDelta = new Vector2(width, height);
    }

    void SetTextPosition()
    {
        Text text = GetComponentInChildren<Text>();
        text.rectTransform.anchoredPosition = new Vector2(TEXT_BORDER, -TEXT_BORDER);

        // Set text contents
        text.text = "";
        int i = 0;
        foreach (string s in m_choices)
        {
            text.text += s;
            i++;
            // Add a new line if this is not the last choice
            if (i != m_choices.Count)
            {
                text.text += "\n";
            }
        }
    }

    // Set the current index of the arrow and move the arrow to the correct position
    void SetArrow(int index)
    {
        if (index >= 0 && index < m_choices.Count)
        {
            m_currentIndex = index;
            SetArrowPosition(index);
        }
    }

    void SetArrowPosition(int index)
    {
        var arrowTransform = m_arrow.GetComponent<RectTransform>();

        // Get height of a line of text
        float lineHeight = GetComponentInChildren<Text>().preferredHeight / m_choices.Count;

        // Calculate position of arrow
        float arrowX = -lineHeight / 2;
        float arrowY = -TEXT_BORDER - (lineHeight * index) - (lineHeight / 2);

        //Set position and size of arrow
        arrowTransform.anchoredPosition = new Vector2(arrowX, arrowY);
        arrowTransform.sizeDelta = new Vector2(lineHeight, lineHeight);
    }
}
