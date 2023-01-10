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
    public Color reviveBarColor;

    [Header("Player colors on revive")]
    public Color[] colors;

    private bool active = true;
    private int reviveCount = 0;

    private List<StatChangeTracker> buffs = new List<StatChangeTracker>();

    private ReversibleColorEffect colorEffect = null;

    private Coroutine buffCoroutine = null;

    private CustomHealthBar reviveBar;
    
    private StatChanges buffToApply = new StatChanges
    {
        Jumps = 1,
        MaxAmmo = 2,
        AttackSpeed = 0.8f,
        MovementSpeed = 1.1f,
        JumpHeight = 1.25f,
        BulletSpeed = 1.25f,
        MaxHealth = 2f,
        Damage = 2f,
    };

    protected override void Start()
    {
        base.Start();
        RevivePatch.RegisterReviveAction(player, ApplyBuff);
        RevivePatch.RegisterTrueDeathAction(player, OnTrueDeath);

        // create and position revive indicator
        reviveBar = new GameObject("Revives Bar", typeof(CustomHealthBar)).GetComponent<CustomHealthBar>();
        player.AddStatusIndicator(reviveBar.gameObject);

        // setup revive indicator
        reviveBar.SetColor(reviveBarColor);
        reviveBar.SetValues(player.data.stats.respawns, player.data.stats.remainingRespawns);
    }

    private void OnTrueDeath()
    {
        reviveBar.CurrentHealth = reviveBar.MaxHealth;

        reviveCount = 0;
        ClearBuffs();
    }

    private void ApplyBuff()
    {
        reviveBar.CurrentHealth = player.data.stats.remainingRespawns - 1;

        reviveCount++;
        buffCoroutine = Unbound.Instance.StartCoroutine(ApplyBuffCoroutine(reviveCount));
    }

    private IEnumerator ApplyBuffCoroutine(int count)
    {
        yield return new WaitUntil(() => player.gameObject.activeInHierarchy);

        int buffCount = buffs.Count;

        ClearBuffs();
        
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
        reviveBar.SetValues(player.data.stats.respawns, player.data.stats.remainingRespawns);
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
        Destroy(reviveBar.gameObject);
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
