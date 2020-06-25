using System;
using GW_Lib;
using UnityEngine;

public class DodgeballLaunchUp : DodgeballAction
{
    public override DodgeballCommand Command => DodgeballCommand.LaunchUp;
    public override string actionName => "Launch Up";

    public event Action onLaunchedUp = null;
    ConstantForce cf = null;

    protected override void Awake()
    {
        base.Awake();
        cf = GetComponent<ConstantForce>();
    }
    
    public void C_LaunchUp(float byHeigth, float launchGravity)
    {
        isRunning = true;
        ball.RunCommand(Command);
        if (!ApplyActionWithCommand())
            return;
        LaunchUp(byHeigth,launchGravity);
    }
    private void LaunchUp(float byHeigth, float launchGravity)
    {
        onLaunchedUp?.Invoke();
        cf.force = Vector3.up * launchGravity;
        this.SetKinematic(false);
        float yVel = Extentions.GetJumpVelocity(byHeigth, cf.force.y);
        rb3d.velocity = Vector3.up * yVel;
    }
}