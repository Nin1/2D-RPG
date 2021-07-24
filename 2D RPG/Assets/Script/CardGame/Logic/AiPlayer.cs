using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AiPlayer
{
    // Fields that the AI will need to know
    /** AI Player's variables */
    public int m_playerID = 1;
    public List<KeyValuePair<int, CardData>> m_cards = new List<KeyValuePair<int, CardData>>();
    public int m_lifeTotal;

    /** Opponent's variables */
    public int m_opponentCardsInHand;
    public int m_opponentLifeTotal;
}
