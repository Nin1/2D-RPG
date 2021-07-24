using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChannelCost : MonoBehaviour {

    [SerializeField]
    Text m_text;

    public void SetCost(string cost)
    {
        m_text.text = cost;
    }

    public void SetCost(int cost)
    {
        m_text.text = cost.ToString();
    }
}
