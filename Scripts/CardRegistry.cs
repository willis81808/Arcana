using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib.Cards;

public static class CardRegistry
{
    private static Dictionary<Type, CardInfo> storedCardInfo = new Dictionary<Type, CardInfo>();

    public static void RegisterCard<T>(bool hidden = false) where T : CustomCard
    {
        CustomCard.BuildCard<T>(c =>
        {
            StoreCard<T>(c);
            if (hidden)
            {
                ModdingUtils.Utils.Cards.instance.AddHiddenCard(c);
            }
        });
    }

    private static void StoreCard<T>(CardInfo card) where T : CustomCard
    {
        storedCardInfo.Add(typeof(T), card);
    }

    public static CardInfo GetCard<T>() where T : CustomCard
    {
        if (storedCardInfo.TryGetValue(typeof(T), out CardInfo value))
        {
            return value;
        }

        return null;
    }

    public static CardInfo GetCard(Type T)
    {
        if (storedCardInfo.TryGetValue(T, out CardInfo value))
        {
            return value;
        }

        return null;
    }
}