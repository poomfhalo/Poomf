using DG.Tweening;
using GW_Lib;
using UnityEngine;

public class DodgeballGoLaunchTo : DodgeballAction
{
    [Tooltip("Time, before ball can collide again, with physics colliders after being thrown")]
    [SerializeField] float leavingHandsTime = 0.1f;

    public override DodgeballCommand Command => DodgeballCommand.LaunchTo;
    public override string actionName => "Thrown To";

    Tweener activeTweener = null;

    public void C_GoLaunchTo(Vector3 targetPos, BallThrowData d)
    {
        ball.lastTargetPos = targetPos;
        ball.lastAppliedThrow = d.id;
        ball.RunCommand(Command);

        if (ApplyActionWithCommand())
        {
            return;
        }

        GoLaunchTo(targetPos, d);
    }
    public void GoLaunchTo(Vector3 targetPos, BallThrowData d)
    {
        isRunning = true;
        scheduler.StartAction(this);
        ball.SetKinematic(true);
        ball.ballState = Dodgeball.BallState.Flying;

        //instance.InvokeDelayed(instance.leavingHandsTime, () => instance.bodyCol.GetCollider.enabled = true);
        float dist = Vector3.Distance(rb3d.position, targetPos);
        float time = d.GetTimeOfDist(dist);
        float tweenV = 0;

        activeTweener = DOTween.To(() => tweenV, f => tweenV = f, 1, time).SetEase(d.ease).OnUpdate(OnUpdate).OnComplete(OnComplete);
        void OnUpdate()
        {
            Vector3 lerpedPos = Vector3.Lerp(ball.position, targetPos, tweenV);
            rb3d.MovePosition(lerpedPos);
        }
        void OnComplete()
        {
            Cancel();
        }
    }
}