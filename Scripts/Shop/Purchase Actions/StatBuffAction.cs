using System;
using ItemShops.Utils;
using ModdingUtils.MonoBehaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModsPlus;
using UnboundLib.Networking;
using Photon.Pun;
using UnboundLib;

[CreateAssetMenu(fileName = "StatBuffAction", menuName = "ItemShop/Actions/StatBuffAction")]
public class StatBuffAction : PurchaseAction
{
    public enum BuffType
    {
        HealthOne,
        HealthTwo,
        DamageOne,
        DamageTwo,
        ReloadOne,
        ReloadTwo,
        BlockOne,
        BlockTwo
    }

    private static Dictionary<BuffType, Type> buffCardMap = new Dictionary<BuffType, Type>
    {
        { BuffType.HealthOne, typeof(HealthOne) },
        { BuffType.HealthTwo, typeof(HealthTwo) },

        { BuffType.DamageOne, typeof(DamageOne) },
        { BuffType.DamageTwo, typeof(DamageTwo) },

        { BuffType.ReloadOne, typeof(ReloadOne) },
        { BuffType.ReloadTwo, typeof(ReloadTwo) },

        { BuffType.BlockOne, typeof(BlockOne) },
        { BuffType.BlockTwo, typeof(BlockTwo) },
    };

    public BuffType buffType;

    public override void OnPurchase(Player player, Purchasable item)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        NetworkingManager.RPC(typeof(StatBuffAction), nameof(RPCA_AddBuffCard), player.playerID, buffType);
    }

    [UnboundRPC]
    private static void RPCA_AddBuffCard(int playerId, BuffType buffType)
    {
        var player = PlayerManager.instance.GetPlayerWithID(playerId);
        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, CardRegistry.GetCard(buffCardMap[buffType]), addToCardBar: true);
    }
}
