using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VideoManager", menuName = "ScriptableObjects/Settings/VideoManager", order = 2)]
public class VideoManager : ScriptableObject, IVideoManager
{
    [SerializeField] private VideoManager defaultVideoSettings;
    public void Initialize()
    {

    }
    public void ResetToDefault()
    {

    }

    #region IVideoManager
    public void UpdateAllSettings(IVideoProvider settings)
    {

    }
    #endregion
}
