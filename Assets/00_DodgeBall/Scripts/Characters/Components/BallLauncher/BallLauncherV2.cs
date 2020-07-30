using System;
using System.Collections;
using GW_Lib;
using UnityEngine;

[RequireComponent(typeof(DodgeballCharacter))]
public class BallLauncherV2 : BallLauncher
{
    [Tooltip("0.1 means, we need to be 10% close enough to the maximum jump heigth, to be able to throw mid air" +
    	"\n0.9 means, we need to be 90% close enough to maximum heigth jump to throw in mid air, meaning, even if we are " +
    	"slightly above ground, then we will be able to throw mid air.")]
    [Range(0.1f,0.9f)]
    [SerializeField] float flyingThrowLaunchRange = 0.2f;

    [Header("Launcher Data")]
    [SerializeField] float throwAtEnemyDelay = 0.3f;

    [Header("Read Only")]
    [SerializeField] bool finishedThrowP1 = false;
    [Tooltip("To be set, from external scripts, mainly used in multiplayer")]
    [SerializeField] bool midAirThrow = false;
    public float travelPercentToThrow = 0;

    Coroutine conditionalThrowCoro = null;
    Coroutine lastSmartWait = null;

    public override void StartThrowAction(DodgeballCharacter toChara)
    {
        if (isThrowing)
            return;

        bool isInAir = !jumper.FeelsGround || jumper.IsJumping;
        bool flyingJumpRangeTest = flyingThrowLaunchRange >= jumper.PosToJumpHeigthPercent;

        if (isInAir)
        {
            if (!flyingJumpRangeTest)
                return;
            midAirThrow = true;
            jumper.StopFlight();
        }
        else
            midAirThrow = false;

        isLastThrowAtEnemy = !TeamsManager.AreFriendlies(chara, toChara);
        finishedThrowP1 = false;
        isThrowing = true;
        aimedAtChara = toChara;

        RunOnBallThrowStart();

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
    protected override void RunOnThrowPointReached()
    {
        base.RunOnThrowPointReached();
        if (midAirThrow)
            jumper.ResumeFlight();
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
        yield return new WaitUntil(() => ExtThrowCondition);
        float endWaitTime = Time.time;
        float externalWaitTime = endWaitTime - startWait;
        if (externalWaitTime < throwAtEnemyDelay)
        {
            float remainingTime = throwAtEnemyDelay - externalWaitTime;
            yield return new WaitForSeconds(remainingTime);
        }
    }
   
    public void A_OnThrowP1Finished()
    {
        if (!aimedAtChara)
            return;
        finishedThrowP1 = true;
    }
    public override void A_OnThrowEnded()
    {
        base.A_OnThrowEnded();
        finishedThrowP1 = false;
    }
}