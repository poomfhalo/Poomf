using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettingsUI : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private Dropdown resolutionDropdown;

    // List of all available resolutions
    Resolution[] resolutions;

    public void Initialize(bool fullscreen, Quality quality, int resolution)
    {
        if (resolutionDropdown.options.Count == 0)
        {
            // Add the available resolutions to the dropdown menu
            resolutions = Screen.resolutions;
            // Prepare the strings needed for adding options into the dropdown menu
            List<string> resolutionsStrings = new List<string>();
            for (int i = 0; i < resolutions.Length; i++)
            {
                resolutionsStrings.Add(resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz");
            }

            // Finally, add the options
            resolutionDropdown.AddOptions(resolutionsStrings);
        }
        UpdateAllUI(fullscreen, quality, resolution);
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
