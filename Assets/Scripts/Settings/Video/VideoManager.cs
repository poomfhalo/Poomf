using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VideoManager", menuName = "ScriptableObjects/Settings/VideoManager", order = 2)]
public class VideoManager : ScriptableObject, IVideoManager
{
    [SerializeField] private bool isFullscreen = false;
    [SerializeField] private Quality quality = Quality.High;

    [SerializeField, HideInInspector] private int resolutionIndex = -1;
    [SerializeField, HideInInspector] private VideoManager defaultSettings;

    // Used to tell if this is the current used settings or not
    public bool isCurrent { set; get; } = false;

    // List of current available resolutions
    private Resolution[] resolutions;
    private void OnEnable()
    {
        Initialize();
    }

    #region ISettingsManager
    public void Initialize()
    {
        resolutions = Screen.resolutions;
        defaultSettings = Resources.Load("Settings/Video/Default Video Settings") as VideoManager;
        SetFullscreen(isFullscreen);
        SetQuality(quality);
        if (resolutionIndex == -1)
        {
            // First time! just set it to the max resolution
            resolutionIndex = resolutions.Length - 1;
        }
        else
        {
            // Update the current resolution
            SetResolution(resolutionIndex);
        }
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
    public void SetResolution(int resolution)
    {
        resolutionIndex = resolution;
        if (isCurrent)
        {
            // The new resolution
            Resolution newRes = resolutions[resolution];
            Screen.SetResolution(newRes.width, newRes.height, isFullscreen);
        }
    }
    public void UpdateAllSettings(IVideoProvider settings)
    {
        SetFullscreen(settings.IsFullscreen());
        SetQuality(settings.GetQualityIndex());
        SetResolution(settings.GetResolutionIndex());
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
    public int GetResolutionIndex()
    {
        return resolutionIndex;
    }
    #endregion
}
