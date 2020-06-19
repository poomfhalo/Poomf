using GW_Lib;
using UnityEngine;

public class DodgeballLaunchUp : DodgeballAction
{
    public override DodgeballCommand Command => DodgeballCommand.LaunchUp;
    public override string actionName => "Launch Up";

    ConstantForce cf = null;

    protected override void Awake()
    {
        base.Awake();
        cf = GetComponent<ConstantForce>();
    }
    void OnEnable()
    {
        ball.E_OnHitGround += OnHitGround;
    }
    void OnDisable()
    {
        ball.E_OnHitGround -= OnHitGround;
    }

    public void C_LaunchUp(float byHeigth, float launchGravity)
    {
        isRunning = true;
        ball.RunCommand(Command);
        if (!ApplyActionWithCommand())
            return;

        LaunchUp(byHeigth,launchGravity);
    }
    public void LaunchUp(float byHeigth, float launchGravity)
    {
        cf.force = Vector3.up * launchGravity;
        this.SetKinematic(false);
        float yVel = Extentions.GetJumpVelocity(byHeigth, cf.force.y);
        rb3d.velocity = Vector3.up * yVel;
    }

    private void OnHitGround()
    {
        if (isRunning)
        {
            Cancel();
        }
    }
}