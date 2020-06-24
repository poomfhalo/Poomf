using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains global audio settings as well as the ability to manage the audio settings
public interface IAudioManager : IAudioProvider, ISettingsManager
{
    void SetSFXVolume(float volume);
    void SetMusicVolume(float volume);
    // Used to change all settings at once
    void UpdateAllSettings(IAudioProvider settings);
}
