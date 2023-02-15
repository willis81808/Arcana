using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using ItemShops.Utils;

[CreateAssetMenu(fileName = "Shop", menuName = "ItemShop/Shop")]
public class ShopObject : ScriptableObject, IProvider<Shop>
{
    public string shopName;

    [AssetSelector, InlineEditor]
    public PurchasableObject[] items;

    [AssetSelector, InlineEditor]
    public PurchasableCardObject[] cards;

    public Shop GetValue()
    {
        var shop = ShopManager.instance.CreateShop(shopName);
        shop.AddItems(items.Select(i => i.GetValue()).ToArray());
        shop.AddItems(cards.Select(c => c.GetValue()).ToArray());
        return shop;
    }
}
