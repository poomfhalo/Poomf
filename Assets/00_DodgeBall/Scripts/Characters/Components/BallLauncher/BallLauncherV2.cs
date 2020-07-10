using System;
using System.Collections;
using GW_Lib;
using UnityEngine;

[RequireComponent(typeof(DodgeballCharacter))]
public class BallLauncherV2 : BallLauncher
{
    [Header("Launcher Data")]
    [SerializeField] float throwDelay = 0.2f;

    [Header("Read Only")]
    [SerializeField] bool finishedThrowPrep = false;
    [Tooltip("To be set, from external scripts, mainly used in multiplayer")]
    public float extDelay = 0;

    Coroutine conditionalThrowCoro = null;

    public override void StartThrowAction(DodgeballCharacter toChara)
    {
        if (isThrowing)
            return;

        finishedThrowPrep = false;
        isThrowing = true;
        aimedAtChara = toChara;

        Action beginThrow = () => {
            animator.SetTrigger("ThrowV2");
            activityName = "Throw ActionV2";
            scheduler.StartAction(this, true);
            this.BeginCoro(ref conditionalThrowCoro, ConditionalThrow());
        };

        if (mover.IsMoving)
            mover.SmoothStop(beginThrow);
        else
            beginThrow.Invoke();
    }
    IEnumerator ConditionalThrow()
    {
        yield return new WaitUntil(() => finishedThrowPrep);
        yield return new WaitForSeconds(throwDelay);
        RunThrowPrepFinished();
        yield return new WaitUntil(()=>ExtThrowCondition());
        yield return new WaitForSeconds(extDelay);
        animator.SetTrigger("ThrowV2");
        Log.LogL0("BallLauncherV2.ConditionalThrow() :: Completed");
    }
    public void A_OnThrowPrepFinished()
    {
        if (!aimedAtChara)
            return;
        finishedThrowPrep = true;
    }
}