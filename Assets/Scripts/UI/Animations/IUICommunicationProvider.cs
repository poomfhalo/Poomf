using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUICommunicationProvider
{
    void ShowScreen(string screenID, AnimationProperties properties = null);
    void HideScreen(string screenID, AnimationProperties properties = null);
    /// <summary>
    /// Adds a waiting period in the animations queue, to put a delay between different animations
    /// </summary>
    void InsertWaitingPeriod(float waitTime);
}
