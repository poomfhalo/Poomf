using System;
using DG.Tweening;
using GW_Lib;
using UnityEngine;

public class DodgeballGoTo : DodgeballAction
{
    [SerializeField] float grabDur = 0.1f;

    public override DodgeballCommand Command => DodgeballCommand.GoToChara;
    public override string actionName => "Goiing To Chara";

    public void C_GoTo(DodgeballCharacter chara, Action onCompleted)
    {
        ball.holder = chara;
        ball.ballState = Dodgeball.BallState.GoingToChara;
        ball.RunCommand(Command);

        if (!ApplyActionWithCommand())
            return;

        GoTo(chara,onCompleted);
    }
    public void GoTo(DodgeballCharacter chara, Action onCompleted)
    {
        scheduler.StartAction(this);

        float currTweener = 0;
        ball.bodyCol.GetCollider.enabled = false;
        ball.SetKinematic(true);

        Vector3 startPos = Dodgeball.instance.position;
        DOTween.To(() => currTweener, f => currTweener = f, 1, grabDur).OnUpdate(OnUpdate).
                                                                        OnComplete(OnComplete).SetEase(Ease.InOutSine);
        void OnUpdate()
        {
            Dodgeball.instance.position = Vector3.Lerp(startPos, chara.BallGrabPoint.position, currTweener);
        }
        void OnComplete()
        {
            ball.ballState = Dodgeball.BallState.Held;

            onCompleted?.Invoke();
            Dodgeball.instance.transform.SetParent(chara.BallGrabPoint);
        }
    }
}