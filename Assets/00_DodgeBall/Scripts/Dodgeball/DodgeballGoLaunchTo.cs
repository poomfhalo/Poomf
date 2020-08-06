﻿using System;
using DG.Tweening;
using GW_Lib;
using UnityEngine;

public class DodgeballGoLaunchTo : DodgeballAction
{
    public event Action onLaunchedTo = null;
    public override DodgeballCommand Command => DodgeballCommand.LaunchTo;
    public override string actionName => "Thrown To";

    public float traveledPercent = 0;

    [Tooltip("Time, before ball can collide again, with physics colliders after being thrown")]
    [SerializeField] float leavingHandsTime = 0.1f;
    [Header("Read Only")]
    [SerializeField] float tweenerVal = 0;
    public Vector3 lastTargetPos = new Vector3();
    public byte lastAppliedThrow = 0;
    public DodgeballCharacter lastThrownAtChara = null;

    Tweener activeTweener = null;

    //TODO: Add The Last character, who threw the ball
    public void C_GoLaunchTo(DodgeballCharacter targetChara,Vector3 targetPos, BallThrowData d)
    {
        lastTargetPos = targetPos;
        lastAppliedThrow = d.id;
        ball.RunCommand(Command);

        if (!ApplyActionWithCommand())
        {
            return;
        }

        GoLaunchTo(targetChara, targetPos, d);
    }
    public void GoLaunchTo(DodgeballCharacter targetChara,Vector3 targetPos, BallThrowData d)
    {
        if (activeTweener != null)
            return;

        this.lastThrownAtChara = targetChara;
        onLaunchedTo?.Invoke();
        isRunning = true;
        scheduler.StartAction(this);
        ball.SetKinematic(true);
        ball.ballState = Dodgeball.BallState.Flying;
        ball.InvokeDelayed(leavingHandsTime, () => ball.bodyCol.GetCollider.enabled = true);
        float dist = Vector3.Distance(rb3d.position, targetPos);
        float time = d.GetTimeOfDist(dist);
        float tweenV = 0;

        Vector3 startPos = ball.position;
        activeTweener = DOTween.To(() => tweenV, f => tweenV = f, 1, time).SetEase(d.ease).OnUpdate(OnUpdate).OnComplete(OnComplete);
        void OnUpdate()
        {
            traveledPercent = tweenV;
            Vector3 lerpedPos = Vector3.Lerp(startPos, targetPos, tweenV);
            rb3d.MovePosition(lerpedPos);
        }
        void OnComplete()
        {
            Log.Message("Ball Completed Its movement");
            this.SetKinematic(false);
            Cancel();
        }
    }
    public override void Cancel()
    {
        base.Cancel();
        if (activeTweener != null)
        {
            activeTweener.Kill();
            activeTweener = null;
        }
        ball.bodyCol.GetCollider.enabled = true;
        isRunning = false;
        traveledPercent = 0;
    }
}