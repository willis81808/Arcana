using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using ItemShops.Utils;
using System;

[CreateAssetMenu(fileName = "PurchasableItem", menuName = "ItemShop/PurchasableItem")]
public class PurchasableObject : ScriptableObject, IProvider<Purchasable>
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

    public GameObject itemPrefab;

    [InlineEditor]
    public PurchaseAction purchaseAction;
    
    public TagObject[] tags;

    public Purchasable GetValue()
    {
        var costDictionary = new Dictionary<string, int>
        {
            { currency, amount }
        };
        return new GenericPurchasable(itemName, costDictionary, tags.Select(t => t.GetValue()).ToArray(), itemPrefab, purchaseAction);
    }
}

public class GenericPurchasable : Purchasable
{
    private string _name;
    private Dictionary<string, int> _cost;
    private Tag[] _tags;
    private GameObject _prefab;
    private PurchaseAction _action;

    public GenericPurchasable(string name, Dictionary<string, int> cost, Tag[] tags, GameObject prefab, PurchaseAction action)
    {
        _name = name;
        _cost = cost;
        _tags = tags;
        _prefab = prefab;
        _action = action;
    }
    
    public override string Name => _name;

    public override Dictionary<string, int> Cost => _cost;

    public override Tag[] Tags => _tags;

    public override bool CanPurchase(Player player)
    {
        return true;
    }

    public override GameObject CreateItem(GameObject parent)
    {
        return GameObject.Instantiate(_prefab, parent.transform);
    }

    public override void OnPurchase(Player player, Purchasable item)
    {
        if (_action == null)
        {
            UnityEngine.Debug.Log($"Player {player.playerID} purchased {Name}");
        }
        else
        {
            _action.OnPurchase(player, item);
        }
    }
}