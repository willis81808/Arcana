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
using System.IO;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnboundLib.Utils;

[BepInDependency("com.willis.rounds.unbound")]
[BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch")]
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
    private const string ModVersion = "1.9.2";
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
        Unbound.AddAllCardsCallback(ApplyBlacklists);
        Unbound.Instance.ExecuteAfterSeconds(3f, DelayedSetup);
    }

    private void ApplyBlacklists(CardInfo[] cards)
    {
        // blacklist justice from decay-like cards
        var justiceCard = AutowiredCard.cards["justice"];
        foreach (var card in cards.Where(c => c != justiceCard && c.GetComponent<CharacterStatModifiers>() is CharacterStatModifiers sm && sm.secondsToTakeDamageOver > 0))
        {
            CustomCardCategories.instance.MakeCardsExclusive(justiceCard, card);
        }

        // blacklist death from phoenix-like cards
        var deathCard = AutowiredCard.cards["death"];
        foreach (var card in cards.Where(c => c != deathCard && c.GetComponent<CharacterStatModifiers>() is CharacterStatModifiers sm && sm.respawns > 0))
        {
            CustomCardCategories.instance.MakeCardsExclusive(deathCard, card);
        }
    }

    private void DelayedSetup()
    {
        // setup fortune shop
        wheelOfFortuneShop = Assets.WheelOfFortuneShop.GetValue();
        wheelOfFortuneShop.itemPurchasedAction += (p, i) =>
        {
            NetworkingManager.RPC(typeof(FortuneHandler), nameof(FortuneHandler.OnItemPurchased));
        };
        UnityEngine.Debug.Log("[Arcana] Created Wheel of Fortune Shop");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            var path = Path.Combine(Path.GetDirectoryName(typeof(ArcanaCardsPlugin).Assembly.Location), "bundle");
            foreach (var part in path.ToString().Split('\\'))
            {
                Logger.LogInfo(part);
            }
        }
    }

    private void SetupMenu(GameObject menu)
    {
        MenuHandler.CreateToggle(reducedParticles.Value, "Reduced Particle Mode", menu, val => reducedParticles.Value = val, 30, false);
    }
}
