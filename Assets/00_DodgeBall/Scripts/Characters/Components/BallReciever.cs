using GW_Lib.Utility;
using UnityEngine;

public class BallReciever : DodgeballCharaAction, ICharaAction
{
    [SerializeField] TriggerDelegator ballRecieveZone = null;
    public string actionName => "Recieve Ball";

    void OnEnable()
    {

    }
    void OnDisable()
    {

    }
    public void StartReciptionAction()
    {
        Debug.Log("reception from team mates, has not been implemented yet, doing nothing");
    }
    public void Cancel()
    {

    }
}