using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ItemShops.Extensions;
using UnboundLib.Networking;
using UnboundLib;

public class FortunesWheel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI resultText;

    [HideInInspector]
    public Player target;

    public void Initialize(Player target)
    {
        this.target = target;
    }

    public void CalculateReward()
    {
        if (target != null && !target.data.view.IsMine) return;

        int winnings = Random.Range(0, 4);
        NetworkingManager.RPC(typeof(FortunesWheel), nameof(RPCA_ShowReward), winnings);
    }

    public void ShowReward(int winnings)
    {
        resultText.text = winnings.ToString();
        resultText.gameObject.SetActive(true);

        if (target == null) return;

        target.GetAdditionalData().bankAccount.Deposit("Fortune", winnings);
    }

    [UnboundRPC]
    private static void RPCA_ShowReward(int winnings)
    {
        GameObject.FindObjectOfType<FortunesWheel>().ShowReward(winnings);
    }
}
