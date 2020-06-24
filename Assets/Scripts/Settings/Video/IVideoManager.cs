using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains the global video settings info as well as the ability to modify them
public interface IVideoManager : IVideoProvider, ISettingsManager
{
    // Updates all settings at once
    void UpdateAllSettings(IVideoProvider settings);
}
