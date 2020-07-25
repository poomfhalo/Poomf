using UnityEngine;
using DG.Tweening;
using System;
using GW_Lib.Utility;
using GW_Lib;

//TODO: based on Dodgeball speed, we increase size of body colliders of characters up to 4, so that, the dodge ball
//does not get missed by smaller colliders, when its going too fast.
public class Dodgeball : Singleton<Dodgeball>
{
    public Func<bool> ExtCanDetectGroundByTrig = () => true;
    public event Action<BallState> E_OnStateUpdated = null;

    public bool IsOnGround => ballState == BallState.OnGround;
    public bool IsHeld => ballState == BallState.Held;
    public bool IsFlying => ballState == BallState.Flying;

    public DodgeballGoLaunchTo launchTo { private set; get; }
    public DodgeballGoTo goTo { private set; get; }
    public DodgeballLaunchUp launchUp { private set; get; }
    public DodgeballReflection reflection { private set; get; }

    public BallState ballState
    {
        get
        {
            return m_ballState;
        }
        set
        {
            if (m_ballState == value)
            {
                //Log.Warning("Trying to set new state to " + value + " but its already assigned");
                return;
            }

            m_ballState = value;
            switch (ballState)
            {
                case BallState.Flying:
                    this.KillCoro(ref delayedGroundedCoro);
                    E_OnStateUpdated?.Invoke(BallState.Flying);
                    break;
                case BallState.Held:
                    this.KillCoro(ref delayedGroundedCoro);
                    E_OnStateUpdated?.Invoke(BallState.Held);
                    break;
                case BallState.OnGround:
                    this.KillCoro(ref delayedGroundedCoro);
                    delayedGroundedCoro = this.InvokeDelayed(timeToGrounded, () => {
                        E_OnStateUpdated?.Invoke(BallState.OnGround);
                    });
                    break;
            }
        }
    }
    public DodgeballCharacter holder => goTo.LastHolder;

    public event Action<DodgeballCommand> E_OnCommandActivated = null;
    public enum BallState { OnGround, Held, Flying }
    public Vector3 position { get { return rb3d.position; } set { rb3d.MovePosition(value); } }

    public CollisionDelegator bodyCol = null;
    [SerializeField] float timeToGrounded = 0.1f;

    [Header("Read Only")]
    [SerializeField] BallState m_ballState = BallState.Flying;

    Coroutine delayedGroundedCoro = null;
    Rigidbody rb3d = null;
    ConstantForce cf = null;
    Vector3 startGravity = Vector3.zero;
    Tweener activeTweener = null;

    protected override void Awake()
    {
        base.Awake();
        rb3d = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
        launchTo = GetComponent<DodgeballGoLaunchTo>();
        goTo = GetComponent<DodgeballGoTo>();
        launchUp = GetComponent<DodgeballLaunchUp>();
        reflection = GetComponent<DodgeballReflection>();

        startGravity = cf.force;
        rb3d.useGravity = false;

        E_OnStateUpdated += (newS) => {
            switch (newS)
            {
                case BallState.Held:
                    bodyCol.GetCollider.enabled = false;
                    break;
                case BallState.Flying:
                    bodyCol.GetCollider.enabled = true;
                    break;
                case BallState.OnGround:
                    bodyCol.GetCollider.enabled = true;
                    break;
            }
        };

        reflection.onReflected += () => bodyCol.GetCollider.enabled = true;
    }
    void OnEnable() => DodgeballGameManager.AddBall(this);
    void OnDisable() => DodgeballGameManager.RemoveBall(this);
    void OnDestroy() => transform.DOKill();
    
    void OnTriggerEnter(Collider col)
    {
        if (!ExtCanDetectGroundByTrig())
            return;

        Field field = col.GetComponent<Field>();
        if (!field)
            return;

        if (ballState == BallState.Flying)
        {
            ballState = BallState.OnGround;
            C_OnGroundHit();
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (!ExtCanDetectGroundByTrig())
            return;

        Field field = col.GetComponent<Field>();
        if (!field)
            return;

        if (ballState == BallState.OnGround)
        {
            ballState = BallState.Flying;
        }
    }

    public void RunCommand(DodgeballCommand command) => E_OnCommandActivated?.Invoke(command);

    private void C_OnGroundHit()
    {
        RunCommand(DodgeballCommand.HitGround);
        if (ExtCanDetectGroundByTrig())
        {
            OnGroundHit();
        }
    }
    public void OnGroundHit()
    {
        ballState = BallState.OnGround;
        DodgeballGameManager.instance.OnBallHitGround();
    }
}