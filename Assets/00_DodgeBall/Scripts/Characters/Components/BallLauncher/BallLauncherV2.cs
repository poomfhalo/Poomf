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
    [SerializeField] bool finishedThrowP1 = false;
    [Tooltip("To be set, from external scripts, mainly used in multiplayer")]
    public float travelPercentToThrow = 0;

    Coroutine conditionalThrowCoro = null;
    Coroutine lastSmartWait = null;

    public override void StartThrowAction(DodgeballCharacter toChara)
    {
        if (isThrowing)
            return;

        finishedThrowP1 = false;
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
        yield return new WaitUntil(() => finishedThrowP1);//wait for animation P1
        RunThrowP1Finished();

        this.BeginCoro(ref lastSmartWait, SmartWait());
        yield return lastSmartWait;

        animator.SetTrigger("ThrowV2");

        float p = 0;//Wait for travel distance
        do
        {
            p = Dodgeball.instance.GetComponent<DodgeballGoLaunchTo>().traveledPercent;
            yield return 0;
        } while (p < travelPercentToThrow);

        RunOnBallLaunchedSafely();
        Log.LogL0("BallLauncherV2.ConditionalThrow() :: Completed");
    }
    IEnumerator SmartWait()//used to at least, wait 0.2 seconds, so if network is faster, we still wait throwDelay
    {
        float startWait = Time.time;
        //Used for networking check, example, master passes, and clients, wait for master to enable them
        yield return new WaitUntil(() => ExtThrowCondition());
        float endWaitTime = Time.time;
        float diff = endWaitTime - startWait;
        if (diff < throwDelay)
        {
            yield return new WaitForSeconds(diff);
        }
    }
    public void A_OnThrowP1Finished()
    {
        if (!aimedAtChara)
            return;
        finishedThrowP1 = true;
    }
}