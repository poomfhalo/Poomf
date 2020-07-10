using System.Collections;
using GW_Lib.Utility.Events;
using UnityEngine;
using UnityEngine.Events;

public class R_CallFunction : Reaction
{
    public enum CallType { Generic, RequireCallBack }
    [Header("Careful, read tooltip")]
    [Tooltip("Warning, if this event, calls a function that does not take an Action call back, then this will be stuck\n" +
        "if this event calls a function, that takes a callback, but the callback fails to be called, Reactor will be stuck")]
    [SerializeField] CallType callType = CallType.Generic;

    public UnityEvent genericFunc = null;

    public UnityCallBackEvent callBackFunc = null;

    bool isDone = false;

    protected override void Initialize()
    {
        isDone = false;
    }
    protected override bool IsDone()
    {
        return isDone;
    }

    protected override IEnumerator ReactionBehavior()
    {
        switch (callType)
        {
            case CallType.Generic:
                genericFunc?.Invoke();
                break;
            case CallType.RequireCallBack:
                bool recievedCallBack = false;
                callBackFunc?.Invoke(() => recievedCallBack = true);
                yield return new WaitUntil(() => recievedCallBack);
                break;
        }
        isDone = true;
    }
}