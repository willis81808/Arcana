using ItemShops.Utils;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DieAction", menuName = "ItemShop/Actions/DieAction")]
public class DieAction : PurchaseAction
{
    public override void OnPurchase(Player player, Purchasable item)
    {
        player.data.view.RPC("RPCA_Die", RpcTarget.All, Vector2.up);
    }
}
