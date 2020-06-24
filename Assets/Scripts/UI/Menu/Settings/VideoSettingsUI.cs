using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettingsUI : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private Dropdown resolutionDropdown;

    public void Initialize(bool fullscreen, Quality qualityIndex)
    {
        fullscreenToggle.isOn = fullscreen;
        qualityDropdown.value = (int)qualityIndex;
    }

    public void UpdateFullscreenToggle(bool value)
    {
        fullscreenToggle.isOn = value;
    }

    public void UpdateQualityDropdownSelection(Quality index)
    {
        qualityDropdown.value = (int)index;
    }
}
