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

[HarmonyPatch(typeof(CardChoice))]
public class UniqueCardChoice_Patch
{
    [HarmonyPatch("SpawnUniqueCard")]
    [HarmonyPostfix]
    public static void SpawnUniqueCard_Patch(ref GameObject __result, int ___pickrID, Vector3 pos, Quaternion rot)
    {
        if (FoolHandler.Instance == null) return;

        var player = PlayerManager.instance.players[___pickrID];

        if (FoolHandler.Instance.owner != player) return;

        // blank out card and display rarity
        var info = __result.GetComponentInChildren<CardInfo>();
        var canvas = info.gameObject.GetComponentInChildren<Canvas>();
        foreach (Transform child in canvas.transform)
        {
            child.localScale = Vector3.zero;
        }
        GameObject.Instantiate(Assets.FoolBlackout, canvas.transform)
            .GetComponent<FoolBlackoutHandler>()
            .Initialize(RarityUtils.GetRarityData(info.rarity));

        // disable card title
        var visuals = __result.GetComponentInChildren<CardVisuals>();
        visuals.nameText.enabled = false;
    }
}
