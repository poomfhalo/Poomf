using System;
using GW_Lib.Utility;
using UnityEngine;

public class BallGrabber : DodgeballCharaAction, ICharaAction
{
    public event Action onBallInHands = null;

    public bool IsBallInGrabZone => isBallInGrabZone;
    public bool HasBall => hasBall;
    public string actionName => activityName;

    [Header("Ball Grabbing")]
    [SerializeField] TriggerDelegator ballGrabbingZone = null;
    [Header("Read Only")]
    [SerializeField] bool isBallInGrabZone = false;
    [SerializeField] string activityName = "Grab Ball";
    [SerializeField] bool hasBall = false;

    Animator animator = null;
    Dodgeball ball = null;

    void Awake()
    {
        ball = Dodgeball.instance;
        animator = GetComponent<Animator>();
        GetComponent<BallLauncher>().onThrowPointReached += () => hasBall = false;

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

    public void GrabBall()
    {
        Dodgeball.GoTo(GetComponent<DodgeballCharacter>(), () => {
            hasBall = true;
            onBallInHands?.Invoke();
        });
        animator.SetBool("HasBall", true);
    }
}