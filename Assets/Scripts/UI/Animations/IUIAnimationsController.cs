using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIAnimationsController
{
    void ShowScreen(AUIAnimatedScreen screen, AnimationProperties properties = null);
    void HideScreen(AUIAnimatedScreen screen, AnimationProperties properties = null);
    /// <summary>
    /// Adds a waiting period in the animations queue, to put a delay between different animations
    /// </summary>
    void InsertWaitingPeriod(float waitTime);
    // Process the queue of pending animations
    IEnumerator ProcessQueue();
    // Adds entries to the queue
    void AddToQueue(IEnumerator process);
    // Stops the current coroutine
    void StopCurrentAnimation();
    /// <summary>
    /// Pause the animations queue, optionally cancels the current animation
    /// </summary>
    /// <param name="cancelCurrent">
    /// set to true to cancel the current animation
    /// </param>
    void PauseAnimationsQueue(bool cancelCurrent = false);
    // Resume the queue
    void ResumeAnimationsQueue();
    void ClearQueue();
}
