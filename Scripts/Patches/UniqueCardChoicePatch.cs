using UnityEngine;
using HarmonyLib;
using UnboundLib;
using UnboundLib.Networking;
using RarityLib.Utils;
using Photon.Pun;

[HarmonyPatch(typeof(CardChoice))]
public class UniqueCardChoicePatch
{
    [HarmonyPatch("SpawnUniqueCard")]
    [HarmonyPostfix]
    public static void SpawnUniqueCard_Patch(ref GameObject __result, int ___pickrID, PickerType ___pickerType, Vector3 pos, Quaternion rot)
    {
        // get currently picking player
        Player player;
        if (___pickerType == PickerType.Team)
        {
            player = PlayerManager.instance.GetPlayersInTeam(___pickrID)[0];
        }
        else
        {
            player = PlayerManager.instance.players[___pickrID];
        }

        // exit if the picking player doesn't have The Fool
        if (!FoolHandler.fooledPlayers.ContainsKey(player)) return;

        // blank out card and display rarity
        SpawnFoolBlackout(__result);

        // network blackout to other players
        NetworkingManager.RPC_Others(
            typeof(UniqueCardChoicePatch),
            nameof(RPC_SpawnFoolBlackout),
            __result.GetComponent<PhotonView>().ViewID
        );
    }

    private static void SpawnFoolBlackout(GameObject card)
    {
        // blank out card and display rarity
        var info = card.GetComponentInChildren<CardInfo>();
        var canvas = info.gameObject.GetComponentInChildren<Canvas>();
        foreach (Transform child in canvas.transform)
        {
            child.localScale = Vector3.zero;
        }
        GameObject.Instantiate(Assets.FoolBlackout, canvas.transform)
            .GetComponent<FoolBlackoutHandler>()
            .Initialize(RarityUtils.GetRarityData(info.rarity));

        // disable card title
        var visuals = card.GetComponentInChildren<CardVisuals>();
        visuals.nameText.enabled = false;
    }

    [UnboundRPC]
    public static void RPC_SpawnFoolBlackout(int targetCardId)
    {
        var card = PhotonNetwork.GetPhotonView(targetCardId).gameObject;
        SpawnFoolBlackout(card);
    }
}