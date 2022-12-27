using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using BepInEx.Configuration;
using UnboundLib.Utils.UI;

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
    private const string ModVersion = "0.0.1";
    private const string CompatabilityModName = "Arcana";

    internal static LayerMask playerMask, projectileMask;
    internal static ConfigEntry<bool> reducedParticles;

    void Awake()
    {
        CustomCard.RegisterUnityCard<DevilCard>(Assets.DevilCard, null);
        CustomCard.RegisterUnityCard<SunCard>(Assets.SunCard, null);
        CustomCard.RegisterUnityCard<HermitCard>(Assets.HermitCard, null);
        CustomCard.RegisterUnityCard<MagicianCard>(Assets.MagicianCard, null);
        CustomCard.RegisterUnityCard<MoonCard>(Assets.MoonCard, null);

        playerMask = LayerMask.GetMask(new string[] { "Player" });
        projectileMask = LayerMask.GetMask(new string[] { "Projectile" });
    }

    void Start()
    {
        reducedParticles = Config.Bind(CompatabilityModName, "Arcana_ReducedParticles", false, "Enable reduced particle mode for faster performance");
        Unbound.RegisterMenu(ModName, null, SetupMenu, showInPauseMenu: true);
    }

    private void SetupMenu(GameObject menu)
    {
        MenuHandler.CreateToggle(reducedParticles.Value, "Reduced Particle Mode", menu, val => reducedParticles.Value = val, 30, false);
    }
}
