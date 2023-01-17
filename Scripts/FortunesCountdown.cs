using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FortunesCountdown : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI countdownText;

    internal static int duration = 60;

    private void Awake()
    {
        FortuneHandler.FinishedPickingEvent.RegisterListener(DoDestroy);
    }

    IEnumerator Start()
    {
        for (int i = 0; i < duration; i++)
        {
            countdownText.text = $"{duration - i}";
            yield return new WaitForSeconds(1);
        }
    }

    private void DoDestroy(FortuneHandler.FinishedPickingEvent e)
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        FortuneHandler.FinishedPickingEvent.UnregisterListener(DoDestroy);
    }
}
