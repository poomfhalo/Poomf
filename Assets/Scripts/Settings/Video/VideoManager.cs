using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VideoManager", menuName = "ScriptableObjects/Settings/VideoManager", order = 2)]
public class VideoManager : ScriptableObject, IVideoManager
{
    // Keeps track of the current display method (fullscreen, windowed, borderless window)
    [SerializeField] private int displayMethodIndex = 0;
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
        SetDisplayMethod(displayMethodIndex);
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
    public void SetDisplayMethod(int value)
    {
        displayMethodIndex = value;
        if (isCurrent)
        {
            if (value == 0)
            {
                // Windowed
                Screen.fullScreen = false;
            }
            else if (value == 1)
            {
                // Exclusive Fullscreen
                Screen.fullScreen = true;
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            }
            else if (value == 2)
            {
                // Borderless Window
                Screen.fullScreen = true;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            else
            {
                Debug.LogError("VideoManager -> SetDisplayMethod: Invalid index!");
            }
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
            // Set the resolution based on the current display mode
            if (displayMethodIndex == 0)
            {
                // Windowed
                Screen.SetResolution(newRes.width, newRes.height, false);
            }
            else if (displayMethodIndex == 1)
            {
                // Exclusive Fullscreen
                Screen.SetResolution(newRes.width, newRes.height, FullScreenMode.ExclusiveFullScreen);
            }
            else if (displayMethodIndex == 2)
            {
                // Borderless Window
                Screen.SetResolution(newRes.width, newRes.height, FullScreenMode.FullScreenWindow);
            }
            ForceUIUpdate();
        }
    }
    public void UpdateAllSettings(IVideoProvider settings)
    {
        SetDisplayMethod(settings.GetDisplayMethod());
        SetQuality(settings.GetQualityIndex());
        SetResolution(settings.GetResolutionIndex());
    }
    public void ForceUIUpdate()
    {
        Canvas.ForceUpdateCanvases();
    }
    #endregion

    #region IVideoProvider
    public int GetDisplayMethod()
    {
        return displayMethodIndex;
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
