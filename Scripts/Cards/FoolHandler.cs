using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using ModdingUtils;
using ModsPlus;
using RarityLib;
using RarityLib.Utils;

public class FoolHandler : PlayerHook
{
    internal static Dictionary<Player, FoolHandler> fooledPlayers = new Dictionary<Player, FoolHandler>();

    [SerializeField]
    private float rarityScalar;

    private Dictionary<Rarity, float> changes = new Dictionary<Rarity, float>();
    
    protected override void Start()
    {
        base.Start();

        fooledPlayers[player] = this;

        if (!player.data.view.IsMine) return;

        var rareData = RarityUtils.GetRarityData(CardInfo.Rarity.Rare);
        foreach (var rarity in RarityUtils.Rarities.Values.Where(r => r.calculatedRarity <= rareData.calculatedRarity))
        {
            UpdateRarity(rarity, rarityScalar);
        }
    }
    
    private void UpdateRarity(Rarity rarity, float multiplier)
    {
        var delta = (rarity.calculatedRarity * multiplier) - rarity.calculatedRarity;

        rarity.calculatedRarity += delta;

        changes.Add(rarity, delta);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        if (!player.data.view.IsMine) return;

        fooledPlayers.Remove(player);

        foreach (var rarity in changes.Keys)
        {
            rarity.calculatedRarity -= changes[rarity];
        }
    }
}
