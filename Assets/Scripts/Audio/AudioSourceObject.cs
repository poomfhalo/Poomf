using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceObject : MonoBehaviour
{
    // The type of this audio source
    [SerializeField]private AudioType type;
    // The settings asset
    [SerializeField]private AudioManager audioSettings;
    private AudioSource audioSource;

    private void OnEnable()
    {
        if(type == AudioType.Music)
            audioSettings.OnMusicVolumeChanged += OnVolumeSettingsChanged;
        else if(type == AudioType.SFX)
            audioSettings.OnSfxVolumeChanged += OnVolumeSettingsChanged;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnVolumeSettingsChanged(float volume)
    {
        audioSource.volume = volume;
    }
}
