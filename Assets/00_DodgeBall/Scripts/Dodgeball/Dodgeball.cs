using UnityEngine;
using DG.Tweening;
using System;
using GW_Lib.Utility;

//TODO: based on Dodgeball speed, we increase size of body colliders of characters up to 4, so that, the dodge ball
//does not get missed by smaller colliders, when its going too fast.
public class Dodgeball : Singleton<Dodgeball>
{
    public Func<bool> CanApplyOnGroundHit = () => true;
    public event Action E_OnHitGround = null;

    public bool IsOnGround => ballState == BallState.OnGround;
    public bool IsHeld => ballState == BallState.Held;
    public bool IsFlying => ballState == BallState.Flying;
    public BallState ballState
    {
        get
        {
            return m_ballState;
        }
        set
        {
            ballState = value;
            TrailRenderer tr = GetComponentInChildren<TrailRenderer>();
            switch (ballState)
            {
                case BallState.Flying:
                    tr.enabled = true;
                    break;
                case BallState.Held:
                    tr.enabled = false;
                    break;
                case BallState.OnGround:
                    tr.enabled = false;
                    break;
            }
        }
    }
    public DodgeballCharacter holder => goTo.LastHolder;

    public event Action<DodgeballCommand> OnCommandActivated = null;
    public enum BallState { OnGround, Held, Flying }
    public Vector3 position { get { return rb3d.position; } set { rb3d.MovePosition(value); } }

    public CollisionDelegator bodyCol = null;
    [Header("Read Only")]
    public byte lastAppliedThrow = 0;
    public Vector3 lastTargetPos = new Vector3();
    [SerializeField] BallState m_ballState = BallState.Flying;

    Rigidbody rb3d = null;
    ConstantForce cf = null;
    Vector3 startGravity = Vector3.zero;
    Tweener activeTweener = null;
    public DodgeballGoLaunchTo launchTo { private set; get; }
    public DodgeballGoTo goTo { private set; get; }
    public DodgeballLaunchUp launchUp { private set; get; }

    protected override void Awake()
    {
        base.Awake();
        rb3d = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
        launchTo = GetComponent<DodgeballGoLaunchTo>();
        goTo = GetComponent<DodgeballGoTo>();
        launchUp = GetComponent<DodgeballLaunchUp>();

        startGravity = cf.force;
        rb3d.useGravity = false;
        bodyCol.onCollisionEnter.AddListener(OnBodyEntered);
        bodyCol.onCollisionEnter.AddListener(OnBodyExitted);
    }
    void OnDestroy()
    {
        transform.DOKill();
    }

    void OnTriggerEnter(Collider col)
    {
        //Field field = col.GetComponent<Field>();
        //if (!field)
        //    return;

        //E_OnHitGround?.Invoke();

        //if(ballState == BallState.Flying)
        //{
        //    ballState = BallState.OnGround;
        //    C_OnGroundHit();
        //}
    }
    void OnTriggerExit(Collider col)
    {
        Field field = col.GetComponent<Field>();
        if (!field)
            return;

        if (ballState == BallState.OnGround)
        {
            ballState = BallState.Flying;
        }
    }
    private void OnBodyEntered(Collision col)
    {

    }
    private void OnBodyExitted(Collision col)
    {

    }
    public void RunCommand(DodgeballCommand command)
    {
        OnCommandActivated?.Invoke(command);
    }
    public void C_OnGroundHit()
    {
        RunCommand(DodgeballCommand.HitGround);
        OnGroundHit();
    }

    public void OnGroundHit()
    {
        if (CanApplyOnGroundHit())
        {
            DodgeballGameManager.instance.OnBallHitGround();
            GetComponentInChildren<TrailRenderer>().enabled = false;
        }
    }
}