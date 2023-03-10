using System.Collections.Generic;
using UnityEngine;
using SoundImplementation;
using UnityEngine.Audio;

public class AudioMixerFixer : MonoBehaviour
{
    public enum AudioType
    {
        SFX, MASTER, MUSIC
    }
    public AudioType type;

    private Dictionary<AudioType, string> groupNames = new Dictionary<AudioType, string>
    {
        { AudioType.MASTER, "MasterPrivate" },
        { AudioType.MUSIC, "MUS" },
        { AudioType.SFX, "SFX" },
    };

    private AudioMixerGroup audioGroup;

    void Awake()
    {
        audioGroup = SoundVolumeManager.Instance.audioMixer.FindMatchingGroups(groupNames[type])[0];

        var player = GetComponent<SoundUnityEventPlayer>();
        if (player != null)
        {
            SetAudioMixerGroup(player.soundStart);
            SetAudioMixerGroup(player.soundStartLoop);
            SetAudioMixerGroup(player.soundEnd);
        }

        var source = GetComponent<AudioSource>();
        if (source != null)
        {
            source.outputAudioMixerGroup = audioGroup;
        }
    }

    private void SetAudioMixerGroup(Sonigon.SoundEvent soundEvent)
    {
        if (soundEvent == null) return;
        soundEvent.variables.audioMixerGroup = audioGroup;
    }
}
