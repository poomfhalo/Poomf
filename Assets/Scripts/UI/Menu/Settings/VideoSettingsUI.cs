using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettingsUI : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private Dropdown resolutionDropdown;

    // An array containing the available resolutions, received from the settings menu on startup
    private List<Resolution> resolutions;

    public void Initialize(IGeneralSettingsProvider settingsProvider)
    {
        if (resolutionDropdown.options.Count == 0)
        {
            // Add the available resolutions to the dropdown menu
            resolutions = settingsProvider.GetResolutionsList();
            // Prepare the strings needed for adding options into the dropdown menu
            List<string> resolutionsStrings = new List<string>();
            for (int i = 0; i < resolutions.Count; i++)
            {
                resolutionsStrings.Add(resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz");
            }

            // Finally, add the options
            resolutionDropdown.AddOptions(resolutionsStrings);
        }
        IVideoProvider videoSettings = settingsProvider.GetVideoSettings();
        UpdateAllUI(videoSettings.IsFullscreen(), videoSettings.GetQualityIndex(), videoSettings.GetResolutionIndex());
    }

    // Updates all UI elements at once
    public void UpdateAllUI(bool fullscreen, Quality quality, int resolution)
    {
        UpdateFullscreenToggle(fullscreen);
        UpdateQualityDropdownSelection(quality);
        UpdateResolutionDropdownSelection(resolution);
    }

    public void UpdateFullscreenToggle(bool value)
    {
        fullscreenToggle.isOn = value;
    }

    public void UpdateQualityDropdownSelection(Quality index)
    {
        qualityDropdown.value = (int)index;
    }

    public void UpdateResolutionDropdownSelection(int index)
    {
        resolutionDropdown.value = index;
    }
}
