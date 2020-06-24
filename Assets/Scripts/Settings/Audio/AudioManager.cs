using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioManager", menuName = "ScriptableObjects/Settings/AudioManager", order = 1)]
public class AudioManager : ScriptableObject, IAudioManager
{
    // An event that's used to notify all audio sources that the music volume has been changed
    public delegate void MusicVolumeChanged(float volume);
    public event MusicVolumeChanged OnMusicVolumeChanged;
    // An event that's used to notify all audio sources that the SFX volume has been changed
    public delegate void SfxVolumeChanged(float volume);
    public event SfxVolumeChanged OnSfxVolumeChanged;
    [SerializeField] private AudioManager defaultSettings;
    // The current volume of SFX audio, which is between 0 and 1
    [SerializeField] private float sfxVolume;
    // The current volume of music audio, which is between 0 and 1
    [SerializeField] private float musicVolume;

    #region ISettingsManager
    public void Initialize()
    {

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
        if (OnSfxVolumeChanged != null)
            OnSfxVolumeChanged(volume);
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
        if (OnMusicVolumeChanged != null)
            OnMusicVolumeChanged(volume);
    }
    public void UpdateAllSettings(IAudioProvider settings)
    {
        SetMusicVolume(settings.GetMusicVolume());
        SetSFXVolume(settings.GetSFXVolume());
    }
    #endregion
}
