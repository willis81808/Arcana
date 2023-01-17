using ItemShops.Utils;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PurchaseAction : ScriptableObject
{
    public abstract void OnPurchase(Player player, Purchasable item);
}
