using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIAnimationsController : IUICommunicationProvider
{
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
}
