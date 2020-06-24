using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VideoManager", menuName = "ScriptableObjects/Settings/VideoManager", order = 2)]
public class VideoManager : ScriptableObject, IVideoManager
{
    [SerializeField] private bool isFullscreen = false;
    [SerializeField] private Quality quality = Quality.Medium;

    [SerializeField, HideInInspector] private VideoManager defaultSettings;

    // Used to tell if this is the current used settings or not
    public bool isCurrent { set; get; } = false;

    #region ISettingsManager
    public void Initialize()
    {
        defaultSettings = Resources.Load("Settings/Video/Default Video Settings") as VideoManager;
    }
    public void ResetToDefault()
    {
        UpdateAllSettings(defaultSettings);
    }
    #endregion

    #region IVideoManager
    public void SetFullscreen(bool fullscreen)
    {
        isFullscreen = fullscreen;
        if (isCurrent)
            Screen.fullScreen = fullscreen;
    }
    public void SetQuality(Quality qualityIndex)
    {
        quality = qualityIndex;
        if (isCurrent)
            QualitySettings.SetQualityLevel((int)qualityIndex);
    }
    public void UpdateAllSettings(IVideoProvider settings)
    {
        SetFullscreen(settings.IsFullscreen());
        SetQuality(settings.GetQualityIndex());
    }
    #endregion

    #region IVideoProvider
    public bool IsFullscreen()
    {
        return isFullscreen;
    }
    public Quality GetQualityIndex()
    {
        return quality;
    }
    #endregion
}
