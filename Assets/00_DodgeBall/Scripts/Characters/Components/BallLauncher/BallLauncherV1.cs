using System;
using UnityEngine;

[RequireComponent(typeof(DodgeballCharacter))]
public class BallLauncherV1 : BallLauncher
{
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
}