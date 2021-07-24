using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGGraveyardLogic {

    List<CGCardObject> m_cards = new List<CGCardObject>();

    public void AddCard(CGCardObject card)
    {
        m_cards.Add(card);
    }
}
