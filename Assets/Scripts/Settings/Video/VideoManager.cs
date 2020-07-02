using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VideoManager", menuName = "ScriptableObjects/Settings/VideoManager", order = 2)]
public class VideoManager : ScriptableObject, IVideoManager
{
    [SerializeField] private bool isFullscreen = false;
    [SerializeField] private Quality quality = Quality.High;

    [SerializeField, HideInInspector] private int resolutionIndex = -1;
    [SerializeField, HideInInspector] private IVideoProvider defaultSettings;

    // Used to tell if this is the current used settings or not
    public bool isCurrent { set; get; } = false;

    // List of current available resolutions
    private List<Resolution> resolutions;

    #region ISettingsManager
    public void Initialize(IGeneralSettingsProvider settingsProvider)
    {
        resolutions = settingsProvider.GetResolutionsList();
        defaultSettings = settingsProvider.GetDefaultVideoSettings();
        SetFullscreen(isFullscreen);
        SetQuality(quality);
        if (resolutionIndex == -1 || resolutionIndex >= resolutions.Count)
        {
            // Either it's the first initialization of this object, or it had an invalid resolution
            // Set it to the current screen resolution
            resolutionIndex = resolutions.IndexOf(Screen.currentResolution);
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
        {
            Screen.fullScreen = fullscreen;
            ForceUIUpdate();
        }
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
            ForceUIUpdate();
        }
    }
    public void UpdateAllSettings(IVideoProvider settings)
    {
        SetFullscreen(settings.IsFullscreen());
        SetQuality(settings.GetQualityIndex());
        SetResolution(settings.GetResolutionIndex());
    }
    public void ForceUIUpdate()
    {
        Canvas.ForceUpdateCanvases();
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
