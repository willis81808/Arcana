using Jotunn.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Assets
{
    internal static AssetBundle Bundle = AssetUtils.LoadAssetBundleFromResources("arcana", typeof(ArcanaCardsPlugin).Assembly);

    internal static ShopObject WheelOfFortuneShop = Bundle.LoadAsset<ShopObject>("Wheel Of Fortune Shop");

    internal static GameObject FoolBlackout = Bundle.LoadAsset<GameObject>("Fool Blackout");
}