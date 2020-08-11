using System;
using UnityEngine;

public class RoundTimer : MonoBehaviour
{
    public event Action E_OnTimerCompleted = null;
    public event Action<float> E_OnTimeUpdated = null;
    [Tooltip("Round Time In Seconds")]
    [SerializeField] float roundTime = 90;
    [Header("Read Only")]
    [SerializeField] float currTime = 0;

    public Func<bool> extDoCount = () => true;
    bool isEntering = true;
    bool didFinish = false;

    void Start()
    {
        currTime = roundTime;
        GameIntroManager.instance.OnEntryCompleted += OnGameStarted;
        isEntering = true;
        didFinish = false;
    }

    private void OnGameStarted()
    {
        isEntering = false;
    }

    void Update()
    {
        if (!extDoCount())
            return;
        if (isEntering)
            return;
        if (didFinish)
            return;

        currTime = currTime - Time.deltaTime;
        E_OnTimeUpdated?.Invoke(currTime);

        if (currTime > 0)
            return;

        didFinish = true;
        E_OnTimerCompleted?.Invoke();
    }
}