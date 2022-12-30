using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using BepInEx.Configuration;
using UnboundLib.Utils.UI;
using HarmonyLib;

[BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.willis.rounds.modsplus", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("root.rarity.lib", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("root.cardtheme.lib", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(ModId, ModName, ModVersion)]
[BepInProcess("Rounds.exe")]
public class ArcanaCardsPlugin : BaseUnityPlugin
{
    private const string ModId = "com.willis.rounds.arcana";
    private const string ModName = "Arcana";
    private const string ModVersion = "1.1.2";
    private const string CompatabilityModName = "Arcana";

    internal static LayerMask playerMask, projectileMask;
    internal static ConfigEntry<bool> reducedParticles;

    void Awake()
    {
        CustomCard.RegisterUnityCard(Assets.DeathCard, "Arcana", "Death", true, null);
        CustomCard.RegisterUnityCard(Assets.SunCard, "Arcana", "The Sun", true, null);
        CustomCard.RegisterUnityCard(Assets.MoonCard, "Arcana", "The Moon", true, null);
        CustomCard.RegisterUnityCard(Assets.DevilCard, "Arcana", "The Devil", true, null);
        CustomCard.RegisterUnityCard(Assets.HermitCard, "Arcana", "The Hermit", true, null);
        CustomCard.RegisterUnityCard(Assets.MagicianCard, "Arcana", "The Magician", true, null);
        
        playerMask = LayerMask.GetMask(new string[] { "Player" });
        projectileMask = LayerMask.GetMask(new string[] { "Projectile" });
    }

    void Start()
    {
        var harmony = new Harmony(ModId);
        harmony.PatchAll();

        reducedParticles = Config.Bind(CompatabilityModName, "Arcana_ReducedParticles", false, "Enable reduced particle mode for faster performance");
        Unbound.RegisterMenu(ModName, null, SetupMenu, showInPauseMenu: true);
    }

    private void SetupMenu(GameObject menu)
    {
        MenuHandler.CreateToggle(reducedParticles.Value, "Reduced Particle Mode", menu, val => reducedParticles.Value = val, 30, false);
    }
}
