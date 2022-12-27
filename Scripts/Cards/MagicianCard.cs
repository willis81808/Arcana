using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModsPlus;
using System.Collections;
using UnboundLib.GameModes;

public class MagicianCard : SimpleCard
{
    public override CardDetails Details => new CardDetails
    {
        Title = "The Magician",
        ModName = "Arcana"
    };

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
    {
        cardInfo.allowMultiple = false;
    }
}
