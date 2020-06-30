using System;
using UnityEngine;

[RequireComponent(typeof(DodgeballCharacter))]
public class BallLauncherV1 : BallLauncher
{
    [Header("Launcher Data")]
    [SerializeField] BallThrowData throwData = null;

    public override void StartThrowAction(DodgeballCharacter toChara)
    {
        if (isThrowing)
            return;

        isThrowing = true;
        aimedAtChara = toChara;
        Action a = () =>{
            animator.SetTrigger("Throw");
            activityName = "Throw Action";
            scheduler.StartAction(this, true);
        };
        if (mover.IsMoving)
            mover.SmoothStop(a);
        else
            a.Invoke();
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