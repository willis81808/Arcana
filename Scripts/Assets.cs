﻿using Jotunn.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Assets
{
    private static AssetBundle Bundle = AssetUtils.LoadAssetBundleFromResources("arcana", typeof(ArcanaCardsPlugin).Assembly);

    internal static GameObject DevilCard = Bundle.LoadAsset<GameObject>("DevilCard");
    internal static GameObject SunCard = Bundle.LoadAsset<GameObject>("SunCard");
    internal static GameObject HermitCard = Bundle.LoadAsset<GameObject>("HermitCard");
    internal static GameObject MagicianCard = Bundle.LoadAsset<GameObject>("MagicianCard");
    internal static GameObject MoonCard = Bundle.LoadAsset<GameObject>("MoonCard");
}