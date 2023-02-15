using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModsPlus;
using UnboundLib.GameModes;
using ItemShops.Extensions;
using UnboundLib.Networking;
using UnboundLib;

public class FortuneHandler : PlayerHook
{
    private bool isShopping = false;

    private static HashSet<Player> trackedPlayers = new HashSet<Player>();

    protected override void Awake()
    {
        base.Awake();
        FinishedPickingEvent.RegisterListener(DonePicking);
    }
    
    public override IEnumerator OnPickPhaseStart(IGameModeHandler gameModeHandler)
    {
        isShopping = false;
        trackedPlayers.Clear();
        yield break;
    }
    
    public override IEnumerator OnPickPhaseEnd(IGameModeHandler gameModeHandler)
    {
        foreach (var player in PlayerManager.instance.players)
        {
            var handler = player.GetComponentInChildren<FortuneHandler>();

            if (handler == null) continue;
            if (trackedPlayers.Contains(player)) continue;
            if (gameModeHandler.GetRoundWinners().Contains(player.playerID)) continue;

            trackedPlayers.Add(player);

            yield return new WaitForSeconds(0.5f);

            // spin the wheel
            var wheel = Instantiate(Assets.Bundle.LoadAsset<GameObject>("Fortune's Wheel").GetComponent<FortunesWheel>(), Unbound.Instance.canvas.transform);
            yield return null;
            wheel.Initialize(player);

            // wait for wheel to stop spinning
            yield return new WaitUntil(() => wheel == null);
            yield return new WaitForSeconds(0.5f);

            // open shop if player has any fortune
            if (player.GetAdditionalData().bankAccount.Money.TryGetValue("Fortune", out int balance) && balance > 0)
            {
                handler.isShopping = true;

                ArcanaCardsPlugin.wheelOfFortuneShop.Show(player);
                if (!player.data.view.IsMine)
                {
                    Instantiate(Assets.Bundle.LoadAsset<GameObject>("Fortune's Countdown"), Unbound.Instance.canvas.transform);
                }

                yield return new WaitForSeconds(0.25f);

                float startTime = Time.time;
                yield return new WaitUntil(() =>
                    !handler.isShopping ||
                    (player.data.view.IsMine && !ArcanaCardsPlugin.wheelOfFortuneShop.IsOpen) ||
                    (Time.time - startTime >= FortunesCountdown.duration)
                );
                NetworkingManager.RPC(typeof(FortuneHandler), nameof(OnItemPurchased), player.playerID);
                ArcanaCardsPlugin.wheelOfFortuneShop.Hide();
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        FinishedPickingEvent.UnregisterListener(DonePicking);
    }

    [UnboundRPC]
    internal static void OnItemPurchased(int playerId)
    {
        var player = ModsPlus.ExtensionMethods.GetPlayerWithID(PlayerManager.instance, playerId);
        new FinishedPickingEvent(player).FireEvent();
    }

    private void DonePicking(FinishedPickingEvent e)
    {
        var self = GetComponentInParent<Player>();
        if (self == null || self != e.target) return;

        isShopping = false;
        ArcanaCardsPlugin.wheelOfFortuneShop.Hide();
    }

    internal class FinishedPickingEvent : EventDispatcher.Event<FinishedPickingEvent>
    {
        public Player target;

        public FinishedPickingEvent(Player target)
        {
            this.target = target;
        }
    }
}
