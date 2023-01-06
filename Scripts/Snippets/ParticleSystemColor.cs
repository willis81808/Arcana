using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemColor : MonoBehaviour
{
    public Color color;

    [SerializeField]
    private ParticleSystem[] particles;
    
    private void Start()
    {
        foreach (var p in particles)
        {
            var module = p.main;
            module.startColor = color;
        }
    }
}
