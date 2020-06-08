using System;
using GW_Lib.Utility;
using UnityEngine;

public class BallCatcher : DodgeballCharaAction, ICharaAction
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
    Dodgeball ball = null;

    void Awake()
    {
        ball = Dodgeball.instance;
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
        if (ball.IsOnGround || ball.IsPostContact)
        {
            GrabBall();
        }
        else
        {
            Debug.LogWarning(ball.ballState + " catching in when ball in this state, has Not been impelemted yet");
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