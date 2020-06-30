﻿using System;
using System.Collections;
using GW_Lib;
using UnityEngine;

[RequireComponent(typeof(DodgeballCharacter))]
public class BallLauncherV2 : BallLauncher
{
    [Header("Launcher Data")]
    [SerializeField] BallThrowData throwData = null;
    [SerializeField] float throwDelay = 0.2f;
    [Header("Read Only")]
    [SerializeField] bool finishedThrowPrep = false;
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
        animator.SetTrigger("ThrowV2");
    }
    public void A_OnThrowPrepFinished()
    {
        if (!aimedAtChara)
            return;
        finishedThrowPrep = true;
    }
    public override void A_OnThrowPointReached()
    {
        if (!aimedAtChara)
            return;
        Dodgeball.instance.transform.SetParent(null);
        animator.SetBool("HasBall", false);
        isThrowing = false;

        Vector3 targetPos = new Vector3();
        if (TeamsManager.AreFriendlies(aimedAtChara, chara))
        {
            //Call, recieving ball or sth here?
            targetPos = aimedAtChara.RecievablePoint.position;
            Debug.LogWarning("Passing To Friendlies has not been implemented yet");
        }
        else
        {
            targetPos = aimedAtChara.ShootablePoint.position;
            Vector3 dir = (targetPos - transform.position).normalized;
            targetPos = targetPos + dir * throwData.ofShootDist;
            Dodgeball.instance.launchTo.C_GoLaunchTo(targetPos, throwData);
            DodgeballGameManager.instance.OnBallThrownAtEnemy(GetComponent<DodgeballCharacter>());
        }

        selectionIndicator.SetFocus(null);
        RunOnThrowPointReached();
    }
}