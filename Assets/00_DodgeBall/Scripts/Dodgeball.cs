﻿using UnityEngine;
using DG.Tweening;
using System;
using GW_Lib;

public class Dodgeball : N_Singleton<Dodgeball>
{
    public static BallState ballState => instance.m_ballState;
    public enum BallState { OnGround,Held,Flying,GoingToChara,PostContact }

    public Vector3 position { get { return transform.position; } set { transform.position = value; } }

    [Tooltip("Time, before ball can collide again, with physics colliders after being thrown")]
    [SerializeField] float leavingHandsTime = 0.1f;
    [SerializeField] float defGrabTime = 0.1f;
    [SerializeField] CollisionDelegator bodyCol = null;

    [Header("Read Only")]
    [SerializeField] BallState m_ballState = BallState.Flying;
#pragma warning disable RECS0122 // Initializing field with default value is redundant
    [SerializeField] bool isCaught = false;
#pragma warning restore RECS0122 // Initializing field with default value is redundant
    [SerializeField] DodgeballCharacter holder = null;
    [SerializeField] float currTweener = 0;

    //TODO: based on Dodgeball speed, we increase size of body colliders of characters up to 4, so that, the dodge ball
    //does not get missed by smaller colliders, when its going too fast.

    //TODO: dynamically find the gravity, depending on distance, such that, speed never exceeds a certain value?

    Rigidbody rb3d = null;
    ConstantForce cf = null;
    Vector3 startGravity = Vector3.zero;

    protected override void Awake()
    {
        base.Awake();
        rb3d = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
        startGravity = cf.force;
        rb3d.useGravity = false;
        bodyCol.onCollisionEnter.AddListener(OnBodyEntered);
        bodyCol.onCollisionEnter.AddListener(OnBodyExitted);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        isCaught = false;
        holder = null;
        currTweener = 0;
    }
    void OnTriggerEnter(Collider col)
    {
        Field field = col.GetComponent<Field>();
        if (!field)
            return;
        if(m_ballState == BallState.Flying || m_ballState == BallState.PostContact)
        {
            m_ballState = BallState.OnGround;
        }
    }
    void OnTriggerExit(Collider col)
    {
        Field field = col.GetComponent<Field>();
        if (!field)
            return;

        if (m_ballState == BallState.OnGround)
        {
            m_ballState = BallState.Flying;
        }
    }

    private void OnBodyEntered(Collision col)
    {

    }
    private void OnBodyExitted(Collision col)
    {
        //m_ballState = BallState.PostContact;
    }

    private static void SetHolder(DodgeballCharacter chara)
    {
        instance.isCaught = true;
        instance.holder = chara;
    }

    public static void GoLaunchTo(DodgeballCharacter chara,Vector3 launchVel,Vector3 gravity, Action onCompleted)
    {
        instance.rb3d.isKinematic = false;
        instance.rb3d.collisionDetectionMode = CollisionDetectionMode.Continuous;
        instance.InvokeDelayed(instance.leavingHandsTime, () => instance.bodyCol.GetCollider.enabled = true);
        instance.rb3d.velocity = launchVel;
        instance.cf.force = gravity;
        instance.m_ballState = BallState.Flying;
    }

    public static void GoTo(DodgeballCharacter chara, Action onCompleted ,float grabTime = -1)
    {
        instance.m_ballState = BallState.GoingToChara;
        float dur = grabTime;

        if (grabTime < 0)
            dur = instance.defGrabTime;

        instance.bodyCol.GetCollider.enabled = false;
        instance.rb3d.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        instance.rb3d.isKinematic = true;

        Vector3 startPos = instance.position;
        DOTween.To(instance.Getter, instance.Setter, 1, dur).OnUpdate(OnUpdate).OnComplete(OnComplete).SetEase(Ease.InOutSine);

        void OnUpdate()
        {
             instance.position = Vector3.Lerp(startPos, chara.BallGrabPoint.position, instance.currTweener);
        }
        void OnComplete()
        {
            SetHolder(chara);
            instance.m_ballState = BallState.Held;

            onCompleted?.Invoke();
            instance.currTweener = 0;
            instance.transform.SetParent(chara.BallGrabPoint);
        }
    }

    private void Setter(float pNewValue)
    {
        instance.currTweener = pNewValue;
    }
    private float Getter()
    {
        return instance.currTweener;
    }
}