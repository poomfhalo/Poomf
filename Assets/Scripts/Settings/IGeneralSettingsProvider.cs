using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Used on global settings classes (like SettingsMenu) to expose certain general purpose settings or variables
/// </summary>
public interface IGeneralSettingsProvider
{
    #region Video
    /// <summary>
    /// Returns a *COPY* of the resolutions list
    /// </summary>
    List<Resolution> GetResolutionsList();
    IVideoProvider GetVideoSettings();
    IVideoProvider GetDefaultVideoSettings();
    #endregion

    #region Audio
    IAudioProvider GetAudioSettings();
    IAudioProvider GetDefaultAudioSettings();
    AudioMixer GetCurrentAudioMixer();
    #endregion
}
