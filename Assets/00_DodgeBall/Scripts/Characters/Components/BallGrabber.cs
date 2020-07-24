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
    Rigidbody rb3d = null;

    void Awake()
    {
        ball = Dodgeball.instance;
        rb3d = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        GetComponent<DodgeballCharacter>().launcher.E_OnThrowPointReached += () => hasBall = false;
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
        if (ball.IsOnGround)
        {
            GrabBall();
        }
        else
        {
            Log.Warning(ball.ballState + " catching in when ball in this state, has Not been impelemted yet");
        }
    }
    public void Cancel()
    {

    }
    public void GrabBall()
    {
        Dodgeball.instance.goTo.C_GoTo(GetComponent<DodgeballCharacter>(), () => {
            hasBall = true;
            onBallInHands?.Invoke();
            rb3d.velocity = Vector3.zero;
        });
        animator.SetBool("HasBall", true);
    }
}