using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModsPlus;
using System.Collections;
using UnboundLib.GameModes;

public class HermitHandler : PlayerHook
{
    public override IEnumerator OnPointEnd(IGameModeHandler gameModeHandler)
    {
        player.gameObject.GetComponentInChildren<TealAura>().ClearBuffs();
        yield break;
    }
}
