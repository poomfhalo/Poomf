using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISettingsManager
{
    void Initialize(IGeneralSettingsProvider settingsProvider);
    void ResetToDefault();
}
