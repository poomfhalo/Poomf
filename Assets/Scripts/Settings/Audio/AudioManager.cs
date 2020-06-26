using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioManager", menuName = "ScriptableObjects/Settings/AudioManager", order = 1)]
public class AudioManager : ScriptableObject, IAudioManager
{
    // The current volume of SFX audio, which is between 0 and 1
    [SerializeField] private float sfxVolume;
    // The current volume of music audio, which is between 0 and 1
    [SerializeField] private float musicVolume;

    // The audio mixer that controls different audio types' settings
    [SerializeField, HideInInspector] private AudioMixer audioMixer;
    [SerializeField, HideInInspector] private IAudioProvider defaultSettings;

    // Used to tell if this is the currently used settings or not
    public bool isCurrent { set; get; } = false;

    // The dB values of the audio, which are used in audio mixers to set volume
    private float sfxVolumedB;
    private float musicVolumedB;

    #region ISettingsManager

    public void Initialize(IGeneralSettingsProvider settingsProvider)
    {
        audioMixer = settingsProvider.GetCurrentAudioMixer();
        defaultSettings = settingsProvider.GetDefaultAudioSettings();
        SetSFXVolume(sfxVolume);
        SetMusicVolume(musicVolume);
    }
    public void ResetToDefault()
    {
        UpdateAllSettings(defaultSettings);
    }
    #endregion

    #region IAudioProvider
    public float GetSFXVolume()
    {
        return sfxVolume;
    }
    public float GetMusicVolume()
    {
        return musicVolume;
    }
    #endregion

    #region IAudioManager
    public void SetSFXVolume(float volume)
    {
        // Volume can only be between 0 and 1
        if (volume < 0)
        {
            // clamp it to 0
            volume = 0;
        }
        else if (volume > 1)
        {
            // clamp to 1
            volume = 1;
        }

        // Update the volume and notify all audio sources that it has been updated
        sfxVolume = volume;
        sfxVolumedB = GetVolumeForMixer(volume);
        if (isCurrent)
            audioMixer.SetFloat("sfxVolume", sfxVolumedB);
    }
    public void SetMusicVolume(float volume)
    {
        // Volume can only be between 0 and 1
        if (volume < 0)
        {
            // clamp it to 0
            volume = 0;
        }
        else if (volume > 1)
        {
            // clamp to 1
            volume = 1;
        }

        // Update the volume and notify all audio sources that it has been updated
        musicVolume = volume;
        musicVolumedB = GetVolumeForMixer(volume);
        if (isCurrent)
            audioMixer.SetFloat("musicVolume", musicVolumedB);
    }
    public void UpdateAllSettings(IAudioProvider settings)
    {
        SetMusicVolume(settings.GetMusicVolume());
        SetSFXVolume(settings.GetSFXVolume());
    }
    #endregion

    // Returns a volume value that can be used with audio mixers [-80dB, 0dB]. Volume should be between 0 and 1
    // Returns a value between [-30dB,0dB]. Using [-80dB,0dB] made the volume drop too rapidly, where it
    // gets too quiet around -30dB, so we use a smaller range instead, and mute it if the volume is 0, which 
    // means returning -80dB.
    float GetVolumeForMixer(float volume)
    {
        if (volume < 0 || volume > 1)
        {
            // Invalid value!
            Debug.LogWarning("Invalid volume in GetVolumeForMixer.");
            return volume;
        }
        else if (volume == 0)
        {
            // Mute
            return -80;
        }
        else
        {
            return Utility.Map(0, 1, -30, 0, volume);
        }
    }
}
