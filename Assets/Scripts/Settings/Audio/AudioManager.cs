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
    [SerializeField, HideInInspector] private AudioManager defaultSettings;

    // Used to tell if this is the currently used settings or not
    public bool isCurrent { set; get; } = false;

    // The dB values of the audio, which are used in audio mixers to set volume
    private float sfxVolumedB;
    private float musicVolumedB;

    private void OnEnable()
    {
        Initialize();
    }

    #region ISettingsManager
    public void Initialize()
    {
        SetSFXVolume(sfxVolume);
        SetMusicVolume(musicVolume);
        audioMixer = Resources.Load("Settings/Audio/AudioMixers/Default") as AudioMixer;
        defaultSettings = Resources.Load("Settings/Audio/Default Audio Settings") as AudioManager;
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

    // TODO: implement in a general helper class
    // Used to map a value in the range [start1 - end1] to the corresponding value
    // In the range [start2 - end2]
    float Map(float start1, float end1, float start2, float end2, float value)
    {
        if (value < start1 || value > end1)
        {
            // The supplied value is out of range
            Debug.LogWarning("Out of range value supplied in Map function.");
            return value;
        }
        else if (start1 == end1 || start2 == end2)
        {
            // Prevent division by 0
            Debug.LogWarning("Invalid range supplied in Map function.");
            return value;
        }
        return start2 + (((value - start1) * (end2 - start2)) / (end1 - start1));
    }

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
            return Map(0, 1, -30, 0, volume);
        }
    }
}
