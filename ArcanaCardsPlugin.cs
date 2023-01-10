﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.Utils.UI;
using ModsPlus;
using UnboundLib.GameModes;

[BepInDependency("com.willis.rounds.unbound")]
[BepInDependency("pykess.rounds.plugins.moddingutils")]
[BepInDependency("com.willis.rounds.modsplus")]
[BepInDependency("root.rarity.lib")]
[BepInDependency("root.cardtheme.lib")]
[BepInDependency("com.root.player.time")]
[BepInPlugin(ModId, ModName, ModVersion)]
[BepInProcess("Rounds.exe")]
public class ArcanaCardsPlugin : BaseUnityPlugin
{
    private const string ModId = "com.willis.rounds.arcana";
    private const string ModName = "Arcana";
    private const string ModVersion = "1.6.1";
    private const string CompatabilityModName = "Arcana";

    internal static LayerMask playerMask, projectileMask, floorMask;
    internal static ConfigEntry<bool> reducedParticles;

    void Awake()
    {
        CustomCard.RegisterUnityCard(Assets.DeathCard, "Arcana", "Death", true, c => c.SetAbbreviation("De"));
        CustomCard.RegisterUnityCard(Assets.SunCard, "Arcana", "The Sun", true, c => c.SetAbbreviation("Su"));
        CustomCard.RegisterUnityCard(Assets.MoonCard, "Arcana", "The Moon", true, c => c.SetAbbreviation("Mo"));
        CustomCard.RegisterUnityCard(Assets.FoolCard, "Arcana", "The Fool", true, c => c.SetAbbreviation("Fo"));
        CustomCard.RegisterUnityCard(Assets.DevilCard, "Arcana", "The Devil", true, c => c.SetAbbreviation("De"));
        CustomCard.RegisterUnityCard(Assets.TowerCard, "Arcana", "The Tower", true, c => c.SetAbbreviation("To"));
        CustomCard.RegisterUnityCard(Assets.HermitCard, "Arcana", "The Hermit", true, c => c.SetAbbreviation("He"));
        CustomCard.RegisterUnityCard(Assets.EmpressCard, "Arcana", "The Empress", true, c => c.SetAbbreviation("Em"));
        CustomCard.RegisterUnityCard(Assets.MagicianCard, "Arcana", "The Magician", true, c => c.SetAbbreviation("Ma"));
        CustomCard.RegisterUnityCard(Assets.TemperenceCard, "Arcana", "Temperance", true, c => c.SetAbbreviation("Te"));
        //CustomCard.RegisterUnityCard(Assets.WheelCard, "Arcana", "Wheel of Fortune", true, c => c.SetAbbreviation("Wh"));
        CustomCard.RegisterUnityCard(Assets.HangedManCard, "Arcana", "The Hanged Man", true, c => c.SetAbbreviation("Ha"));
        CustomCard.RegisterUnityCard(Assets.HierophantCard, "Arcana", "The Hierophant", true, c => c.SetAbbreviation("Hi"));
        //CustomCard.RegisterUnityCard(Assets.HighPriestessCard, "Arcana", "The High Priestess", true, c => c.SetAbbreviation("Pr"));

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
    }

    private void SetupMenu(GameObject menu)
    {
        MenuHandler.CreateToggle(reducedParticles.Value, "Reduced Particle Mode", menu, val => reducedParticles.Value = val, 30, false);
    }
}
