using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HarmonyLib;
using RarityLib.Utils;

internal class FoolBlackoutHandler : MonoBehaviour
{
    [SerializeField]
    private Image colorBorder;

    [SerializeField]
    private TextMeshProUGUI rarityText;

    private Rarity rarity;

    internal void Initialize(Rarity rarity)
    {
        this.rarity = rarity;
    }

    void Start()
    {
        DisplayRarity();
    }

    private void DisplayRarity()
    {
        var color = rarity == RarityUtils.GetRarityData(CardInfo.Rarity.Common) ? Color.white : rarity.color;
        colorBorder.color = color;
        rarityText.color = color;
        rarityText.text = rarity.name;
    }
}
