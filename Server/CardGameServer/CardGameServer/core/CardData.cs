using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CardData : ScriptableObject
{
    public string cardName = "NewCard";
    public int channelCost = 0;
    public int cooldownCost = 0;
    public CardType cardType = CardType.SPELL;
    public string rulesText = "";
    public bool hasTargets = false;
    public string cardScriptName;
    public List<CardAttribute> attributes;
}
