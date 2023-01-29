using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModsPlus;
using HarmonyLib;
using System;
using System.Linq;

public class JusticeHandler : PlayerHook
{
    [SerializeField]
    private float thornsFactor;

    [SerializeField]
    private GameObject retributionEffect;

    protected override void Start()
    {
        base.Start();
        TakeDamagePatch.RegisterTakeDamageAction(player, OnTakeDamage);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TakeDamagePatch.DeregisterTakeDamageAction(player, OnTakeDamage);
    }

    private void OnTakeDamage(Vector2 damage, Player damagingPlayer)
    {
        if (player.teamID == damagingPlayer.teamID) return;

        damagingPlayer.data.healthHandler.TakeDamage(GetScaledDamage(), damagingPlayer.transform.position, damagingPlayer: player);
        var particles = Instantiate(retributionEffect, damagingPlayer.transform);
        particles.transform.localPosition = Vector3.zero;
        particles.transform.localScale *= 2f;
    }

    private Vector2 GetScaledDamage()
    {
        UnityEngine.Debug.Log($"Max Health: {player.data.maxHealth}\nThorns factor: {thornsFactor}\nCalculated damage: {(Vector2.one * player.data.maxHealth * thornsFactor).magnitude}\n\n");
        return Vector2.one * player.data.maxHealth * thornsFactor;
    }
}


[HarmonyPatch(typeof(HealthHandler))]
public class TakeDamagePatch
{
    private static Dictionary<Player, Action<Vector2, Player>> registeredDamageActions = new Dictionary<Player, Action<Vector2, Player>>();

    [HarmonyPatch("DoDamage")]
    [HarmonyPostfix]
    public static void PhoenixRevive_Postfix(HealthHandler __instance, Vector2 damage, Player damagingPlayer)
    {
        if (damagingPlayer == null) return;
        if (__instance.GetComponent<Player>() is Player self && registeredDamageActions.ContainsKey(self))
        {
            registeredDamageActions[self]?.Invoke(damage, damagingPlayer);
        }
    }
    
    public static void RegisterTakeDamageAction(Player player, Action<Vector2, Player> onRevive)
    {
        if (registeredDamageActions.TryGetValue(player, out var action))
        {
            action += onRevive;
        }
        else
        {
            registeredDamageActions.Add(player, onRevive);
        }
    }

    public static void DeregisterTakeDamageAction(Player player, Action<Vector2, Player> onRevive)
    {
        if (registeredDamageActions.TryGetValue(player, out var action))
        {
            action -= onRevive;
        }
    }
}
