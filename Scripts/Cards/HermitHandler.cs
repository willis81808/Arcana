using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModsPlus;
using System.Collections;
using UnboundLib.GameModes;
using UnboundLib;

public class HermitHandler : PlayerHook
{
    public override IEnumerator OnBattleStart(IGameModeHandler gameModeHandler)
    {
        var effect = player.gameObject.GetComponentInChildren<TealAura>();
        effect.StopAllCoroutines();
        effect.ClearBuffs();

        Unbound.Instance.ExecuteAfterFrames(1, () => effect.StartCoroutine(effect.AuraEffect()));

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
