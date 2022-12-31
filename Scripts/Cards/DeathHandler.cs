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

    private ReversibleColorEffect colorEffect = null;

    private Coroutine buffCoroutine = null;

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
        RevivePatch.RegisterReviveAction(player, ApplyBuff);
        RevivePatch.RegisterTrueDeathAction(player, OnTrueDeath);
    }

    private void OnTrueDeath()
    {
        UnityEngine.Debug.Log("True death");

        reviveCount = 0;
        ClearBuffs();
    }

    private void ApplyBuff()
    {
        UnityEngine.Debug.Log("Revive");

        reviveCount++;
        buffCoroutine = Unbound.Instance.StartCoroutine(ApplyBuffCoroutine(reviveCount));
    }

    private IEnumerator ApplyBuffCoroutine(int count)
    {
        yield return new WaitUntil(() => player.gameObject.activeInHierarchy);

        int buffCount = buffs.Count;

        ClearBuffs();

        UnityEngine.Debug.Log($"Removed {buffCount} buffs, adding {count} new buffs");

        yield return null;

        colorEffect = player.gameObject.AddComponent<ReversibleColorEffect>();
        var color = count - 1 < colors.Length ? colors[count - 1] : colors[colors.Length - 1];
        colorEffect.SetColor(color);

        for (int i = 0; i < count; i++)
        {
            var appliedBuff = StatManager.Apply(player, buffToApply);
            buffs.Add(appliedBuff);
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
        ClearColor();
        foreach (var buff in buffs.Where(b => b != null))
        {
            StatManager.Remove(buff);
        }
        buffs.Clear();
    }

    private void ClearColor()
    {
        if (colorEffect != null)
        {
            Destroy(colorEffect);
            colorEffect = null;
        }
    }

    protected override void OnDestroy()
    {
        RevivePatch.DeregisterReviveAction(player, ApplyBuff);
        RevivePatch.DeregisterTrueDeathAction(player, OnTrueDeath);
        ClearBuffs();
        base.OnDestroy();
    }
}

[HarmonyPatch(typeof(HealthHandler))]
public class RevivePatch
{
    private static Dictionary<Player, Action> registeredReviveActions = new Dictionary<Player, Action>();
    private static Dictionary<Player, Action> registeredTrueDeathActions = new Dictionary<Player, Action>();
    
    [HarmonyPatch("RPCA_Die_Phoenix")]
    [HarmonyPrefix]
    public static void PhoenixRevive_Postfix(Player ___player, bool ___isRespawning, Vector2 deathDirection)
    {
        if (___isRespawning || ___player.data.dead) return;

        if (registeredReviveActions.TryGetValue(___player, out Action onRevive))
        {
            onRevive?.Invoke();
        }
    }

    [HarmonyPatch("RPCA_Die")]
    [HarmonyPrefix]
    public static void TrueDeath_Postfix(Player ___player, bool ___isRespawning, Vector2 deathDirection)
    {
        if (___isRespawning || ___player.data.dead) return;

        if (registeredTrueDeathActions.TryGetValue(___player, out Action onTrueDeath))
        {
            onTrueDeath?.Invoke();
        }
    }

    public static void RegisterReviveAction(Player player, Action onRevive)
    {
        if (registeredReviveActions.TryGetValue(player, out Action action))
        {
            action += onRevive;
        }
        else
        {
            registeredReviveActions.Add(player, onRevive);
        }
    }

    public static void DeregisterReviveAction(Player player, Action onRevive)
    {
        if (registeredReviveActions.TryGetValue(player, out Action action))
        {
            action -= onRevive;
        }
    }

    public static void RegisterTrueDeathAction(Player player, Action onTrueDeath)
    {
        if (registeredTrueDeathActions.TryGetValue(player, out Action action))
        {
            action += onTrueDeath;
        }
        else
        {
            registeredTrueDeathActions.Add(player, onTrueDeath);
        }
    }

    public static void DeregisterTrueDeathAction(Player player, Action onTrueDeath)
    {
        if (registeredTrueDeathActions.TryGetValue(player, out Action action))
        {
            action -= onTrueDeath;
        }
    }
}
