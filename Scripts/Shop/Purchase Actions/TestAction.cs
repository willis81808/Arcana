using ItemShops.Utils;
using ModsPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestAction", menuName = "ItemShop/Actions/TestAction")]
public class TestAction : PurchaseAction
{
    public override void OnPurchase(Player player, Purchasable item)
    {
        StatManager.Apply(player, new StatChanges
        {
            MaxHealth = 5f
        });
    }
}
