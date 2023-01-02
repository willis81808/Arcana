using System.Collections;
using System.Linq;
using UnboundLib.GameModes;
using UnityEngine;

public class TempExtraPicks : MonoBehaviour
{
    [SerializeField]
    private int extraPicks;
    
    internal static IEnumerator HandleExtraPicks()
    {
        foreach (Player player in PlayerManager.instance.players.ToArray())
        {
            var extraDrawComponents = player.GetComponentsInChildren<TempExtraPicks>();

            var remainingDraws = extraDrawComponents.Sum(e => e.extraPicks);
            if (remainingDraws <= 0) continue;

            extraDrawComponents.Where(e => e.extraPicks > 0).First().extraPicks -= 1;            

            yield return GameModeManager.TriggerHook(GameModeHooks.HookPlayerPickStart);
            CardChoiceVisuals.instance.Show(Enumerable.Range(0, PlayerManager.instance.players.Count).Where(i => PlayerManager.instance.players[i].playerID == player.playerID).First(), true);
            yield return CardChoice.instance.DoPick(1, player.playerID, PickerType.Player);
            yield return new WaitForSecondsRealtime(0.1f);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookPlayerPickEnd);
            yield return new WaitForSecondsRealtime(0.1f);
        }
        yield break;
    }
}
