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
    public override IEnumerator OnBattleStart(IGameModeHandler gameModeHandler)
    {
        var effect = player.gameObject.GetComponentInChildren<TealAura>();
        effect.ClearBuffs();
        yield break;
    }

    public override IEnumerator OnPointEnd(IGameModeHandler gameModeHandler)
    {
        var effect = player.gameObject.GetComponentInChildren<TealAura>();
        effect.StopAllCoroutines();
        effect.ClearBuffs();
        yield break;
    }
}
