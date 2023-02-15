using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using ItemShops.Utils;
using System;

[CreateAssetMenu(fileName = "PurchasableCardItem", menuName = "ItemShop/PurchasableCardItem")]
public class PurchasableCardObject : ScriptableObject, IProvider<PurchasableCard>
{
    [LabelWidth(75)]
    [HorizontalGroup("Item Details")]
    public string itemName;

    [LabelWidth(75)]
    [HorizontalGroup("Item Details")]
    public string itemId;

    [LabelWidth(75)]
    [HorizontalGroup("Currency Details")]
    public string currency;

    [LabelWidth(75)]
    [HorizontalGroup("Currency Details")]
    public int amount;

    public CardProvider card;

    public TagObject[] tags;
    
    public PurchasableCard GetValue()
    {
        var costDictionary = new Dictionary<string, int>
        {
            { currency, amount }
        };
        return new PurchasableCard(card.GetValue(), costDictionary, tags.Select(t => t.GetValue()).ToArray());
    }
}
