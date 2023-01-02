using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardAddedEvent : OnAddEffect
{
    public UnityEvent onAdded;

    public override void OnAdd(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
    {
        onAdded?.Invoke();
    }
}
