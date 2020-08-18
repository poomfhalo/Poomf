using System;
using DG.Tweening;
using GW_Lib;
using UnityEngine;

public class DodgeballGoLaunchTo : DodgeballAction
{
    public event Action onLaunchedTo = null;
    public override DodgeballCommand Command => DodgeballCommand.LaunchTo;
    public override string actionName => "Thrown To";

    public float traveledPercent = 0;

    [Header("Read Only")]
    [SerializeField] float tweenerVal = 0;
    public Vector3 lastTargetPos = new Vector3();
    public byte lastAppliedThrow = 0;
    public DodgeballCharacter lastThrownAtChara = null;
    public DodgeballCharacter lastThrower = null;

    Tweener activeTweener = null;

    public void C_GoLaunchTo(DodgeballCharacter lastThrower, DodgeballCharacter targetChara,Vector3 targetPos)
    {
        this.lastThrower = lastThrower;
        lastTargetPos = targetPos;
        lastAppliedThrow = GetComponent<DodgeballThrowSetter>().GetLastSelectedThrowData().id;
        ball.RunCommand(Command);

        if (!ApplyActionWithCommand())
        {
            return;
        }

        GoLaunchTo(lastThrower,targetChara, targetPos);
    }
    public void GoLaunchTo(DodgeballCharacter lastThrower,DodgeballCharacter targetChara,Vector3 targetPos)
    {
        if (activeTweener != null)
            return;

        this.lastThrownAtChara = targetChara;
        this.lastThrower = lastThrower;
        onLaunchedTo?.Invoke();
        isRunning = true;
        scheduler.StartAction(this);
        ball.SetKinematic(true);
        ball.ballState = Dodgeball.BallState.Flying;
        ball.bodyCol.GetCollider.enabled = false;
        float dist = Vector3.Distance(rb3d.position, targetPos);
        BallThrowData d = GetComponent<DodgeballThrowSetter>().GetLastSelectedThrowData();
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
            rb3d.velocity = d.GetSpeed() * (targetPos - startPos).normalized;
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