using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.Utils.UI;
using ModsPlus;
using UnboundLib;
using UnboundLib.GameModes;
using ItemShops.Utils;
using ItemShops.Extensions;
using UnboundLib.Networking;

[BepInDependency("com.willis.rounds.unbound")]
[BepInDependency("pykess.rounds.plugins.moddingutils")]
[BepInDependency("com.willuwontu.rounds.itemshops")]
[BepInDependency("com.willis.rounds.modsplus")]
[BepInDependency("com.root.player.time")]
[BepInDependency("root.cardtheme.lib")]
[BepInDependency("root.rarity.lib")]
[BepInPlugin(ModId, ModName, ModVersion)]
[BepInProcess("Rounds.exe")]
public class ArcanaCardsPlugin : BaseUnityPlugin
{
    private const string ModId = "com.willis.rounds.arcana";
    private const string ModName = "Arcana";
    private const string ModVersion = "1.7.1";
    private const string CompatabilityModName = "Arcana";

    internal static LayerMask playerMask, projectileMask, floorMask;
    internal static ConfigEntry<bool> reducedParticles;

    internal static Shop wheelOfFortuneShop;

    void Awake()
    {
        // register arcana cards
        AutowiredCard.RegisterAll(Assets.Bundle);

        // register hidden cards
        CardRegistry.RegisterCard<HealthOne>(true);
        CardRegistry.RegisterCard<DamageOne>(true);
        CardRegistry.RegisterCard<HealthTwo>(true);
        CardRegistry.RegisterCard<DamageTwo>(true);
        CardRegistry.RegisterCard<ReloadOne>(true);
        CardRegistry.RegisterCard<ReloadTwo>(true);
        CardRegistry.RegisterCard<BlockOne>(true);
        CardRegistry.RegisterCard<BlockTwo>(true);

        GameModeManager.AddHook(GameModeHooks.HookPlayerPickEnd, (gm) => TempExtraPicks.HandleExtraPicks());

        playerMask = LayerMask.GetMask(new string[] { "Player" });
        projectileMask = LayerMask.GetMask(new string[] { "Projectile" });
        floorMask = LayerMask.GetMask(new string[] { "Default", "IgnorePlayer" });
    }

    void Start()
    {
        var harmony = new Harmony(ModId);
        harmony.PatchAll();

        reducedParticles = Config.Bind(CompatabilityModName, "Arcana_ReducedParticles", false, "Enable reduced particle mode for faster performance");
        Unbound.RegisterMenu(ModName, null, SetupMenu, showInPauseMenu: true);

        Unbound.Instance.ExecuteAfterSeconds(3f, () =>
        {
            wheelOfFortuneShop = Assets.WheelOfFortuneShop.GetValue();
            UnityEngine.Debug.Log("Created Wheel of Fortune Shop");

            wheelOfFortuneShop.itemPurchasedAction += (p, i) =>
            {
                NetworkingManager.RPC(typeof(FortuneHandler), nameof(FortuneHandler.OnItemPurchased));
            };
        });
    }
    
    private void SetupMenu(GameObject menu)
    {
        MenuHandler.CreateToggle(reducedParticles.Value, "Reduced Particle Mode", menu, val => reducedParticles.Value = val, 30, false);
    }
}
