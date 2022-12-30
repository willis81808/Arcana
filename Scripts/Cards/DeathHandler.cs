using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnboundLib;
using ModdingUtils.MonoBehaviours;
using ModsPlus;
using System.Collections;
using UnboundLib.GameModes;
using HarmonyLib;

public class DeathHandler : PlayerHook
{
    public Color[] colors;

    private bool active = true;
    private int reviveCount = 0;

    private List<StatChangeTracker> buffs = new List<StatChangeTracker>();

    public StatChanges buffToApply = new StatChanges
    {
        Damage = 2f,
        MaxHealth = 2f,
        AttackSpeed = 0.5f,
        MovementSpeed = 1.5f,
        BulletSpeed = 2f,
        Jumps = 2,
        MaxAmmo = 3
    };

    protected override void Start()
    {
        base.Start();
        RevivePatch.RegisterAction(player, ApplyBuff);
    }

    private void ApplyBuff(bool isFullRevive)
    {
        ClearBuffs();

        if (isFullRevive)
        {
            reviveCount = 0;
            return;
        }
        if (!active) return;


        reviveCount++;
        Unbound.Instance.StartCoroutine(ApplyBuffCoroutine(reviveCount));
    }

    private IEnumerator ApplyBuffCoroutine(int count)
    {
        yield return new WaitUntil(() => player.gameObject.activeInHierarchy);

        var colorEffect = player.gameObject.AddComponent<ReversibleColorEffect>();
        var color = count - 1 < colors.Length ? colors[count - 1] : colors[colors.Length - 1];
        colorEffect.SetColor(color);

        for (int i = 0; i < count; i++)
        {
            var appliedBuff = StatManager.Apply(player, buffToApply);
            buffs.Add(appliedBuff);
            yield return null;
        }
    }

    public override IEnumerator OnBattleStart(IGameModeHandler gameModeHandler)
    {
        ClearBuffs();
        active = true;
        reviveCount = 0;
        yield break;
    }

    public override IEnumerator OnPointEnd(IGameModeHandler gameModeHandler)
    {
        active = false;
        ClearBuffs();
        reviveCount = 0;
        yield break;
    }

    private void ClearBuffs()
    {
        foreach (var buff in buffs.Where(b => b != null))
        {
            StatManager.Remove(buff);
        }
        buffs.Clear();
    }
    
    protected override void OnDestroy()
    {
        RevivePatch.DeregisterAction(player, ApplyBuff);
        ClearBuffs();
        base.OnDestroy();
    }
}

[HarmonyPatch(typeof(HealthHandler))]
public class RevivePatch
{
    private static Dictionary<Player, Action<bool>> registeredReviveActions = new Dictionary<Player, Action<bool>>();

    [HarmonyPatch("Revive")]
    [HarmonyPostfix]
    public static void Revive_Postfix(HealthHandler __instance, Player ___player, bool isFullRevive)
    {
        if (registeredReviveActions.TryGetValue(___player, out Action<bool> onRevive))
        {
            onRevive?.Invoke(isFullRevive);
        }
    }

    public static void RegisterAction(Player player, Action<bool> onRevive)
    {
        if (registeredReviveActions.TryGetValue(player, out Action<bool> action))
        {
            action += onRevive;
        }
        else
        {
            registeredReviveActions.Add(player, onRevive);
        }
    }

    public static void DeregisterAction(Player player, Action<bool> onRevive)
    {
        if (registeredReviveActions.TryGetValue(player, out Action<bool> action))
        {
            action -= onRevive;
        }
    }
}
