using System;
using GW_Lib.Utility;
using UnityEngine;

public class BallCatcher : MonoBehaviour,ICharaAction
{
    public event Action onBallInHands = null;

    public bool IsBallInGrabZone => isBallInGrabZone;
    public string actionName => activityName;

    [Header("Ball Grabbing")]
    [SerializeField] TriggerDelegator ballGrabbingZone = null;
    [Header("Read Only")]
    [SerializeField] bool isBallInGrabZone = false;
    [SerializeField] string activityName = "Grab Ball";

    Animator animator = null;

    void Awake()
    {
        animator = GetComponent<Animator>();

        ballGrabbingZone.onTriggerEnter.AddListener(OnBallGrabZoneEntered);
        ballGrabbingZone.onTriggerExit.AddListener(OnBallGrabZoneExitted);
    }

    private void OnBallGrabZoneEntered(Collider col)
    {
        if (!col.GetComponent<Dodgeball>())
            return;

        isBallInGrabZone = true;
    }
    private void OnBallGrabZoneExitted(Collider col)
    {
        if (!col.GetComponent<Dodgeball>())
            return;

        isBallInGrabZone = false;
    }
    public void StartCatchAction()
    {
        switch (Dodgeball.instance.ballState)
        {
            case Dodgeball.BallState.OnGround:
                GrabBall();
                break;
            case Dodgeball.BallState.PostContact:
                GrabBall();
                break;
            default:
                Debug.LogWarning(Dodgeball.instance.ballState + " catching in when ball in this state, has Not been impelemted yet");
                break;
        }
        if (Dodgeball.instance.ballState == Dodgeball.BallState.OnGround || Dodgeball.instance.ballState == Dodgeball.BallState.PostContact)
        {
            GrabBall();
        }
    }
    public void Cancel()
    {

    }

    private void GrabBall()
    {
        Dodgeball.GoTo(GetComponent<DodgeballCharacter>(), () => {
            onBallInHands?.Invoke();
        });
        animator.SetBool("HasBall", true);
    }
}