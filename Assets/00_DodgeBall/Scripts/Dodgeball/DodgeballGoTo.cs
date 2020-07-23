using System;
using DG.Tweening;
using GW_Lib;
using UnityEngine;

public class DodgeballGoTo : DodgeballAction
{
    public event Action onGoto = null;
    public override DodgeballCommand Command => DodgeballCommand.GoToChara;
    public override string actionName => "Goiing To Chara";
    public DodgeballCharacter LastHolder => lastHolder;

    [SerializeField] float grabDur = 0.1f;
    [SerializeField] float heldPosCheckerDur = 0.025f;
    [Header("Read Only")]
    [SerializeField] DodgeballCharacter lastHolder = null;

    void OnDisable()
    {
        transform.DOKill();
    }

    public void C_GoTo(DodgeballCharacter chara, Action onCompleted)
    {
        lastHolder = chara;
        ball.RunCommand(Command);

        if (!ApplyActionWithCommand())
            return;

        GoTo(chara,onCompleted);
    }
    public void GoTo(DodgeballCharacter chara, Action onCompleted)
    {
        ball.ballState = Dodgeball.BallState.Held;
        onGoto?.Invoke();
        isRunning = true;
        scheduler.StartAction(this);
        Debug.Log("Starting This Action?");

        float currTweener = 0;
        ball.bodyCol.GetCollider.enabled = false;
        ball.SetKinematic(true);

        Vector3 startPos = Dodgeball.instance.position;
        DOTween.To(() => currTweener, f => currTweener = f, 1, grabDur).OnUpdate(OnUpdate).
                                                                        OnComplete(OnComplete).SetEase(Ease.InOutSine);
        void OnUpdate()
        {
            if (!ball)
                return;
            ball.position = Vector3.Lerp(startPos, chara.BallGrabPoint.position, currTweener);
        }
        void OnComplete()
        {
            if (!ball)
                return;

            onCompleted?.Invoke();
            ball.transform.SetParent(chara.BallGrabPoint);
            Cancel();
            ball.transform.DOLocalMove(Vector3.zero, heldPosCheckerDur).SetEase(Ease.InOutSine);
        }
    }
    public override void Cancel()
    {
        base.Cancel();
        transform.DOKill();
    }
}