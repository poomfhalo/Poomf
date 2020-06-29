using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimationsController : MonoBehaviour, IUIAnimationsController
{
    //private Dictionary<string, IUIAnimatedScreenController> screens = new Dictionary<string, IUIAnimatedScreenController>();
    [SerializeField] private AUIAnimatedScreen settingsAnimatedScreen;
    // The queue that holds the pending animations
    private Queue<IEnumerator> animationsQueue = new Queue<IEnumerator>();
    // The currently running coroutine
    private IEnumerator currentCoroutine = null;
    // Flag that's raised when we want to pause the queue
    private bool pauseQueue = false;

    #region Setters/Getters
    public AUIAnimatedScreen SettingsAnimatedScreen { get { return settingsAnimatedScreen; } private set { settingsAnimatedScreen = value; } }
    #endregion

    private void Awake()
    {
        // Initialize the global screens
        settingsAnimatedScreen.Initialize();
    }
    private void Start()
    {

        // Start processing the queue, make sure pause is false
        pauseQueue = false;
        StartCoroutine(ProcessQueue());
    }

    private IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
    #region IUIAnimationsController
    public void ShowScreen(AUIAnimatedScreen screen, AnimationProperties properties = null)
    {
        AddToQueue(screen.AnimateIn(properties));
    }
    public void HideScreen(AUIAnimatedScreen screen, AnimationProperties properties = null)
    {
        AddToQueue(screen.AnimateOut(properties));
    }
    public void InsertWaitingPeriod(float waitTime)
    {
        AddToQueue(Wait(waitTime));
    }


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
        currentCoroutine = null;
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
    public void ClearQueue()
    {
        animationsQueue.Clear();
    }
    #endregion
}
