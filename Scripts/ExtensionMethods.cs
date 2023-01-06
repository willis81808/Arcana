using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool IsFullyAlive(this Player p)
    {
        return !p.data.dead && !p.data.healthHandler.isRespawning;
    }

    public static bool IsMinion(this Player p)
    {
        return ModdingUtils.AIMinion.Extensions.CharacterDataExtension.GetAdditionalData(p.data).isAIMinion;
    }
}
