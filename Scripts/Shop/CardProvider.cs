using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using ItemShops.Utils;
using System;

[CreateAssetMenu(fileName = "CardProvider", menuName = "ItemShop/CardProvider")]
public class CardProvider: ScriptableObject, IProvider<CardInfo>
{
    public enum CardType
    {
        HealthOne,
        HealthTwo,
        DamageOne,
        DamageTwo,
        ReloadOne,
        ReloadTwo,
        BlockOne,
        BlockTwo,
        AmmoOne,
        AmmoTwo
    }

    private static Dictionary<CardType, Type> cardMap = new Dictionary<CardType, Type>
    {
        { CardType.HealthOne, typeof(HealthOne) },
        { CardType.HealthTwo, typeof(HealthTwo) },

        { CardType.DamageOne, typeof(DamageOne) },
        { CardType.DamageTwo, typeof(DamageTwo) },

        { CardType.ReloadOne, typeof(ReloadOne) },
        { CardType.ReloadTwo, typeof(ReloadTwo) },

        { CardType.BlockOne, typeof(BlockOne) },
        { CardType.BlockTwo, typeof(BlockTwo) },

        { CardType.AmmoOne, typeof(AmmoOne) },
        { CardType.AmmoTwo, typeof(AmmoTwo) },
    };

    public CardType card;
    
    public CardInfo GetValue()
    {
        return CardRegistry.GetCard(cardMap[card]);
    }
}
