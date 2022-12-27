using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModsPlus;
using System.Collections;
using UnboundLib.GameModes;

public class HermitCard : CustomEffectCard<HermitHandler>
{
    public override CardDetails Details => new CardDetails
    {
        Title = "The Hermit",
        ModName = "Arcana"
    };

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
    {
        cardInfo.allowMultiple = false;
    }
}

public class HermitHandler : CardEffect
{
    public override IEnumerator OnPointEnd(IGameModeHandler gameModeHandler)
    {
        player.gameObject.GetComponentInChildren<TealAura>().ClearBuffs();
        yield break;
    }
}
