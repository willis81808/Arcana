using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModsPlus;

public class DevilCard : SimpleCard
{
    public override CardDetails Details => new CardDetails
    {
        Title = "The Devil",
        ModName = "Arcana"
    };

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
    {
        cardInfo.allowMultiple = false;
    }
}