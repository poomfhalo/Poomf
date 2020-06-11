﻿using UnityEngine;
using DG.Tweening;
using System;
using GW_Lib.Utility;

//TODO: based on Dodgeball speed, we increase size of body colliders of characters up to 4, so that, the dodge ball
//does not get missed by smaller colliders, when its going too fast.
//TODO: dynamically find the gravity, depending on distance, such that, speed never exceeds a certain value?
public class Dodgeball : Singleton<Dodgeball>
{
    public event Action OnHitGround = null;

    public bool IsOnGround => ballState == BallState.OnGround;
    public bool IsHeld => ballState == BallState.Held;
    public bool IsFlying => ballState == BallState.Flying;
    public bool IsGoingToChara => ballState == BallState.GoingToChara;
    public bool IsPostContact => ballState == BallState.PostContact;

    public event Action<DodgeballCommand> OnCommandActivated = null;
    public enum BallState { OnGround, Held, Flying, GoingToChara, PostContact }
    public Vector3 position { get { return rb3d.position; } set { rb3d.MovePosition(value); } }

    public CollisionDelegator bodyCol = null;
    [Header("Read Only")]
    public DodgeballCharacter holder = null;
    public byte lastAppliedThrow = 0;
    public Vector3 lastTargetPos = new Vector3();
    [Header("Synced Variables")]
    public BallState ballState = BallState.Flying;

    Rigidbody rb3d = null;
    ConstantForce cf = null;
    Vector3 startGravity = Vector3.zero;
    Tweener activeTweener = null;
    public DodgeballGoLaunchTo launchTo { private set; get; }
    public DodgeballGoTo goTo { private set; get; }
    public DodgeballLaunchUp launchUp { private set; get; }

    public DodgeballCharacter GetHolder()
    {
        return holder;
    }

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
        holder = null;
        transform.DOKill();
    }

    void OnTriggerEnter(Collider col)
    {
        Field field = col.GetComponent<Field>();
        if (!field)
            return;
        OnHitGround?.Invoke();
        if(ballState == BallState.Flying || ballState == BallState.PostContact)
        {
            ballState = BallState.OnGround;
        }
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
        //m_ballState = BallState.PostContact;
    }

    public void RunCommand(DodgeballCommand command)
    {
        OnCommandActivated?.Invoke(command);
    }
}