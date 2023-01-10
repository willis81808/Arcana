using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BepInEx;
using UnityEngine.Events;

public class ParticleReducer : MonoBehaviour
{
    public UnityEvent onReduceParticles;

    void Start()
    {
        if (ArcanaCardsPlugin.reducedParticles.Value)
        {
            onReduceParticles?.Invoke();
        }
    }
}
