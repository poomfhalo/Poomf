﻿using UnityEngine;
using DG.Tweening;
using System;
using GW_Lib.Utility;
using GW_Lib;

//TODO: based on Dodgeball speed, we increase size of body colliders of characters up to 4, so that, the dodge ball
//does not get missed by smaller colliders, when its going too fast.
public class Dodgeball : Singleton<Dodgeball>
{
    public Func<bool> CanApplyOnGroundHit = () => true;
    public event Action E_OnGroundedAfterTime = null;

    public bool IsOnGround => ballState == BallState.OnGround;
    public bool IsHeld => ballState == BallState.Held;
    public bool IsFlying => ballState == BallState.Flying;

    public DodgeballGoLaunchTo launchTo { private set; get; }
    public DodgeballGoTo goTo { private set; get; }
    public DodgeballLaunchUp launchUp { private set; get; }

    public BallState ballState
    {
        get
        {
            return m_ballState;
        }
        set
        {
            m_ballState = value;
            TrailRenderer tr = GetComponentInChildren<TrailRenderer>();
            switch (ballState)
            {
                case BallState.Flying:
                    if (timedGroundedCoro != null)
                    {
                        Debug.LogWarning("XXXX");
                        StopCoroutine(timedGroundedCoro);
                        timedGroundedCoro = null;
                    }
                    tr.enabled = true;
                    break;
                case BallState.Held:
                    if(timedGroundedCoro != null)
                    {
                        StopCoroutine(timedGroundedCoro);
                        timedGroundedCoro = null;
                    }
                    tr.enabled = false;
                    break;
                case BallState.OnGround:
                    if(timedGroundedCoro != null)
                    {
                        Debug.LogWarning("stopped, old timed event thing?");
                        StopCoroutine(timedGroundedCoro);
                        timedGroundedCoro = null;
                    }
                    timedGroundedCoro = this.InvokeDelayed(timeToGrounded, () => {
                        tr.enabled = false;
                        E_OnGroundedAfterTime?.Invoke();
                        Log.Message("OnGrounded After Time");
                    });
                    break;
            }
        }
    }
    public DodgeballCharacter holder => goTo.LastHolder;

    public event Action<DodgeballCommand> OnCommandActivated = null;
    public enum BallState { OnGround, Held, Flying }
    public Vector3 position { get { return rb3d.position; } set { rb3d.MovePosition(value); } }

    public CollisionDelegator bodyCol = null;
    [SerializeField] float timeToGrounded = 0.1f;

    [Header("Read Only")]
    public byte lastAppliedThrow = 0;
    public Vector3 lastTargetPos = new Vector3();
    [SerializeField] BallState m_ballState = BallState.Flying;

    Coroutine timedGroundedCoro = null;
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

        startGravity = cf.force;
        rb3d.useGravity = false;
        bodyCol.onCollisionEnter.AddListener(OnBodyEntered);
        bodyCol.onCollisionEnter.AddListener(OnBodyExitted);
    }
    void OnEnable()
    {
        DodgeballGameManager.AddBall(this);
    }
    void OnDisable()
    {
        DodgeballGameManager.RemoveBall(this);
    }
    void OnDestroy()
    {
        transform.DOKill();
    }
    void OnTriggerEnter(Collider col)
    {
        if (!CanApplyOnGroundHit())
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
        if (!CanApplyOnGroundHit())
            return;

        Field field = col.GetComponent<Field>();
        if (!field)
            return;

        if (ballState == BallState.OnGround)
        {
            ballState = BallState.Flying;
        }
    }
    private void OnBodyEntered(Collision col){ }
    private void OnBodyExitted(Collision col){ }

    public void RunCommand(DodgeballCommand command) => OnCommandActivated?.Invoke(command);

    public void C_OnGroundHit()
    {
        RunCommand(DodgeballCommand.HitGround);
        OnGroundHit();
    }

    public void OnGroundHit()
    {
        if (CanApplyOnGroundHit())
        {
            ballState = BallState.OnGround;
            DodgeballGameManager.instance.OnBallHitGround();
        }
    }
}