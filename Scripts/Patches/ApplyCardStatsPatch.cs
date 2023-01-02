using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnboundLib;

[HarmonyPatch(typeof(ApplyCardStats), "ApplyStats")]
internal class ApplyCardStatsPatch
{
    private static void Postfix(ApplyCardStats __instance, Player ___playerToUpgrade)
    {
        __instance.GetComponentsInChildren<OnAddEffect>().ToList().ForEach(effect => { effect.Run(___playerToUpgrade); });
    }
}