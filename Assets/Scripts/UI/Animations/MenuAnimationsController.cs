using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimationsController : MonoBehaviour, IUIAnimationsController
{
    [SerializeField] Dictionary<string, IUIAnimatedScreenController> screens = new Dictionary<string, IUIAnimatedScreenController>();

    // The queue that holds the pending animations
    private Queue<IEnumerator> animationsQueue = new Queue<IEnumerator>();
    // The currently running coroutine
    private IEnumerator currentCoroutine;
    // Flag that's raised when we want to pause the queue
    private bool pauseQueue = false;

    private void Start()
    {
        // Start processing the queue, make sure pause is false
        pauseQueue = false;
        StartCoroutine(ProcessQueue());
    }

    public void RegisterScreen(string screenID, IUIAnimatedScreenController screen)
    {
        screens.Add(screenID, screen);
    }

    private IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

    #region IUICommmunicationProvider
    public void ShowScreen(string screenID, AnimationProperties properties = null)
    {
        AddToQueue(screens[screenID].AnimateIn(properties));
    }
    public void HideScreen(string screenID, AnimationProperties properties = null)
    {
        AddToQueue(screens[screenID].AnimateOut(properties));
    }
    public void InsertWaitingPeriod(float waitTime)
    {
        AddToQueue(Wait(waitTime));
    }
    #endregion

    #region IUIAnimationsController
    public IEnumerator ProcessQueue()
    {
        while (true && !pauseQueue)
        {
            // If the queue has entries, start one of them and wait till it's done
            if (animationsQueue.Count > 0)
            {
                currentCoroutine = animationsQueue.Dequeue();
                yield return StartCoroutine(currentCoroutine);
            }
            else
            {
                // Remain idle
                yield return null;
            }
        }
    }
    public void AddToQueue(IEnumerator process)
    {
        animationsQueue.Enqueue(process);
    }
    public void StopCurrentAnimation()
    {
        StopCoroutine(currentCoroutine);
    }
    public void PauseAnimationsQueue(bool cancelCurrent = false)
    {
        if (cancelCurrent)
            StopCurrentAnimation();

        pauseQueue = true;
    }
    public void ResumeAnimationsQueue()
    {
        pauseQueue = false;
    }
    #endregion
}
