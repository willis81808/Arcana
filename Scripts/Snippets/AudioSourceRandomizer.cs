using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceRandomizer : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;

    private void Awake()
    {
        if (clips.Length <= 0) return;

        var source = GetComponent<AudioSource>();
        source.clip = clips.GetRandom<AudioClip>();
    }
}
