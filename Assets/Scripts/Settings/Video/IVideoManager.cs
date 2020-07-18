using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains the global video settings info as well as the ability to modify them
public interface IVideoManager : IVideoProvider, ISettingsManager
{
    void SetDisplayMethod(int value);
    void SetQuality(Quality qualityIndex);
    void SetResolution(int index);
    // Updates all settings at once
    void UpdateAllSettings(IVideoProvider settings);
    // Forces all canvas menus to update after video settings are changed
    void ForceUIUpdate();
}
