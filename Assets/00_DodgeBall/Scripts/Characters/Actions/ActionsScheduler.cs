using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actions scheduler, a script to sequence the actions that a character does, so that, no more than one action
/// is being played at the same time.
/// For Convience it is recommended, any Function, that is intended, to be active as an "Action" then,
/// the function must start by Start word, example StartMoveByInput will start the movement action on the Mover
/// </summary>
public class ActionsScheduler : MonoBehaviour
{
    public string CurrActionName => currActionName;
    [SerializeField] string currActionName = "";

    [SerializeField] List<string> actions = new List<string>();
    ICharaAction currAction = null;

    public void StartAction(ICharaAction action,bool cancelPrevious = true)
    {
        if (action == currAction)
            return;
        if (action == null)
            currActionName = "";
        if (currAction != null && cancelPrevious)
            currAction.Cancel();

        currAction = action;
        currActionName = action.actionName;
        if (actions.Count > 20)
            actions.Clear();
        actions.Add(currAction.actionName);
    }
}