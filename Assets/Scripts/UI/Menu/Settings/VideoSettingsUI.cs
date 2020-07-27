using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettingsUI : MonoBehaviour
{
    [SerializeField] private Dropdown displayDropdown = null;
    [SerializeField] private Dropdown qualityDropdown = null;
    [SerializeField] private Dropdown resolutionDropdown = null;

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
                resolutionsStrings.Add(resolutions[i].width + " x " + resolutions[i].height);
            }

            // Finally, add the options
            resolutionDropdown.AddOptions(resolutionsStrings);
        }
        IVideoProvider videoSettings = settingsProvider.GetVideoSettings();
        UpdateAllUI(videoSettings.GetDisplayMethod(), videoSettings.GetQualityIndex(), videoSettings.GetResolutionIndex());
    }

    // Updates all UI elements at once
    public void UpdateAllUI(int displayMethod, Quality quality, int resolution)
    {
        UpdateDisplayDropDownSelection(displayMethod);
        UpdateQualityDropdownSelection(quality);
        UpdateResolutionDropdownSelection(resolution);
    }

    public void UpdateDisplayDropDownSelection(int index)
    {
        displayDropdown.value = index;

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
