using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib.GameModes;
using ModsPlus;
using ModdingUtils.MonoBehaviours;

public class TemperanceHandler : PlayerHook
{
    public const float BUFF_PERCENTAGE = 0.5f;
    public const float DEBUFF_PERCENTAGE = 0.1f;

    private List<ReversibleEffect> appliedEffects = new List<ReversibleEffect>();

    public override IEnumerator OnBattleStart(IGameModeHandler gameModeHandler)
    {
        yield return null;
        
        int myScore = -1;
        int bestScoreOthers = -1;

        foreach (var teamId in PlayerManager.instance.players.Select(p => p.teamID).Distinct())
        {
            if (teamId == player.teamID)
            {
                myScore = gameModeHandler.GetTeamScore(teamId).rounds; 
            }
            else
            {
                var score = gameModeHandler.GetTeamScore(teamId).rounds;
                bestScoreOthers = bestScoreOthers > score ? bestScoreOthers : score;
            }
        }

        if (myScore == bestScoreOthers)
        {
            ApplyEffect<AheadBuff>();
            ApplyEffect<BehindBuff>();
        }
        else if (myScore < bestScoreOthers)
        {
            ApplyEffect<BehindBuff>(Mathf.Abs(myScore - bestScoreOthers));
        }
        else
        {
            ApplyEffect<AheadBuff>(Mathf.Abs(myScore - bestScoreOthers));
        }
    }

    private void ApplyEffect<T>(int count = 1) where T : ReversibleEffect
    {
        for (int i = 0; i < count; i++)
        {
            var buff = player.gameObject.AddComponent<T>();
            appliedEffects.Add(buff);
        }
    }
    
    private void ClearEffects()
    {
        foreach (var e in appliedEffects)
        {
            Destroy(e);
        }
        appliedEffects.Clear();
    }

    public override IEnumerator OnPointEnd(IGameModeHandler gameModeHandler)
    {
        ClearEffects();
        yield break;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ClearEffects();
    }
}

internal class AheadBuff : ReversibleEffect
{
    public override void OnStart()
    {
        SetLivesToEffect(int.MaxValue);
        characterDataModifier.maxHealth_mult = 1 - TemperanceHandler.DEBUFF_PERCENTAGE;
        gunStatModifier.damage_mult = 1 - TemperanceHandler.DEBUFF_PERCENTAGE;
        blockModifier.cdMultiplier_mult = 1 + TemperanceHandler.DEBUFF_PERCENTAGE;
        gunAmmoStatModifier.reloadTimeMultiplier_mult = 1 + TemperanceHandler.DEBUFF_PERCENTAGE;
    }
}

internal class BehindBuff : ReversibleEffect
{
    public override void OnStart()
    {
        SetLivesToEffect(int.MaxValue);
        characterDataModifier.maxHealth_mult = 1 + TemperanceHandler.BUFF_PERCENTAGE;
        gunStatModifier.damage_mult = 1 + TemperanceHandler.BUFF_PERCENTAGE;
        blockModifier.cdMultiplier_mult = 1 - TemperanceHandler.BUFF_PERCENTAGE;
        gunAmmoStatModifier.reloadTimeMultiplier_mult = 1 - TemperanceHandler.BUFF_PERCENTAGE;
    }
}
