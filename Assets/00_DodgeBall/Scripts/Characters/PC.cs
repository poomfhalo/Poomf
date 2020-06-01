using System;
using GW_Lib.Utility;
using UnityEngine;

public class PC : DodgeballCharacter
{
    void OnEnable()
    {
        MatchInputController.OnMoveInput += OnMoveInput;
        MatchInputController.OnCatch += OnCatch;
        MatchInputController.OnEnemy += OnEnemy;
        MatchInputController.OnFriendly += OnFriendly;
        MatchInputController.OnFire += OnFire;
        MatchInputController.OnDodge+= OnDodge;
        MatchInputController.OnFakeFire += OnFakeFire;
        MatchInputController.OnJump += OnJump;
    }
    void OnDisable()
    {
        MatchInputController.OnMoveInput -= OnMoveInput;
        MatchInputController.OnCatch -= OnCatch;
        MatchInputController.OnEnemy -= OnEnemy;
        MatchInputController.OnFriendly -= OnFriendly;
        MatchInputController.OnFire -= OnFire;
        MatchInputController.OnDodge -= OnDodge;
        MatchInputController.OnFakeFire -= OnFakeFire;
        MatchInputController.OnJump -= OnJump;
    }

    private void OnMoveInput(Vector3 i)
    {
        if (jumper.IsJumping)
        {
            jumper.UpdateInput(i);
            return;
        }
        mover.StartMoveByInput(i, cam.transform);
    }
    private void OnCatch()
    {
        if (HasBall)
            return;
        if (!IsBallInGrabZone)
            return;

        catcher.StartCatchAction();
    }
    private void OnFriendly()
    {
        if(HasBall)
        {
            selectionIndicator.SetNewFocus(true);
        }
    }
    private void OnEnemy()
    {
        if (HasBall)
        {
            selectionIndicator.SetNewFocus(false);
        }
    }
    private void OnFire()
    {
        if (IsThrowing)
            return;
        if (!HasBall)
            return;
        launcher.UpdateInput(mover.input);
        launcher.StartThrowAction(selectionIndicator.ActiveSelection);
    }
    private void OnDodge()
    {
        if (HasBall)
            return;
        if (IsThrowing)
            return;
        if (IsDodging)
            return;

        dodger.StartDodgeAction();
    }
    private void OnFakeFire()
    {
        if (!HasBall)
            return;
        if (IsThrowing)
            return;
        launcher.UpdateInput(mover.input);
        launcher.StartFakeThrow(selectionIndicator.ActiveSelection);
    }
    private void OnJump()
    {
        if (HasBall)
            return;
        if (IsThrowing)
            return;
        if (IsDodging)
            return;
        if (IsJumping)
            return;
        jumper.StartJumpAction();

    }
}