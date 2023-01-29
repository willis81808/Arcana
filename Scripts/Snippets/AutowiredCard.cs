using UnityEngine;
using Sirenix.OdinInspector;
using UnboundLib.Cards;
using ModsPlus;
using System.Collections.Generic;

public class AutowiredCard : MonoBehaviour
{
    internal static Dictionary<string, CardInfo> cards = new Dictionary<string, CardInfo>();

    [SerializeField]
    private string modName;

    [SerializeField]
    [ValidateInput("MaxLength", "\"$cardAbbreviation\" is longer than 2 characters", InfoMessageType.Error)]
    private string cardAbbreviation;

    private bool MaxLength(string value)
    {
        return value.Length <= 2;
    }

    public void Register()
    {
        CustomCard.RegisterUnityCard(gameObject, modName, GetComponent<CardInfo>().cardName, true, c =>
        {
            c.SetAbbreviation(cardAbbreviation);
            cards.Add(c.cardName.ToLower(), c);
        });
    }

    public static void RegisterAll(AssetBundle bundle)
    {
        foreach (var go in bundle.LoadAllAssets<GameObject>())
        {
            if (go.GetComponent<AutowiredCard>() is AutowiredCard aw)
            {
                aw.Register();
            }
        }
    }
}
