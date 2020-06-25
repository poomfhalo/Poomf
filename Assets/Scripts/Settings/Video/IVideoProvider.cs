using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains info about global video settings but doesn't allow them to be manipulated
public interface IVideoProvider
{
    bool IsFullscreen();
    Quality GetQualityIndex();
    int GetResolutionIndex();
}
