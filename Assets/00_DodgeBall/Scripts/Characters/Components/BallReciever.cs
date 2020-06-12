﻿using System;
using GW_Lib.Utility;
using UnityEngine;

public class BallReciever : DodgeballCharaAction, ICharaAction
{
    public event Action onBallGrabbed = null;
    public Func<bool> CanDetectBall = () => true;
    public bool IsDetecting => isDetecting;

    [SerializeField] TriggerDelegator ballRecieveZone = null;

    [Header("Read Only")]
    [SerializeField] bool isBallIn = false;
    [SerializeField] bool isDetecting = false;

    public string actionName => "Recieve Ball";

    void OnEnable()
    {
        GetComponent<CharaHitPoints>().OnHPUpdated += OnHPUpdated;
        ballRecieveZone.onTriggerEnter.AddListener(OnEntered);
        ballRecieveZone.onTriggerExit.AddListener(OnExitted);
    }
    void OnDisable()
    {
        ballRecieveZone.onTriggerEnter.RemoveListener(OnEntered);
        ballRecieveZone.onTriggerExit.RemoveListener(OnExitted);
    }
    void Update()
    {
        if (!isDetecting)
            return;
        if (!CanDetectBall())
            return;

        Bounds b = ballRecieveZone.GetCollider.bounds;
        Collider[] overlaps = Physics.OverlapBox(b.center, b.extents);
        foreach (var col in overlaps)
        {
            if (col.GetComponent<Dodgeball>())
                SetIsBallIn(true);
        }
    }

    private void OnEntered(Collider other)
    {
        if (!CanDetectBall())
            return;

        Dodgeball ball = other.GetComponent<Dodgeball>();

        if (!ball)
            return;

        SetIsBallIn(true);
    }
    private void OnExitted(Collider other)
    {
        if (!CanDetectBall())
            return;

        Dodgeball ball = other.GetComponent<Dodgeball>();

        if (!ball)
            return;

        SetIsBallIn(false);
    }
    private void OnHPUpdated()
    {
        DisableDetection();
    }

    public void EnableDetection()
    {
        isDetecting = true;
    }
    public void DisableDetection()
    {
        isDetecting = false;
    }
    public void Cancel()
    {

    }

    private void SetIsBallIn(bool state)
    {
        if(isBallIn != state)
        {
            isBallIn = state;
            if(state)
            {
                TryGrabBall();
            }
        }
    }
    public void TryGrabBall()
    {
        if(isBallIn && isDetecting && isButtonClicked)
        {
            GetComponent<DodgeballCharacter>().C_ReleaseFromBrace();
            GetComponent<BallGrabber>().GrabBall();
            DisableDetection();
            isButtonClicked = false;
            onBallGrabbed?.Invoke();
        }
    }
}