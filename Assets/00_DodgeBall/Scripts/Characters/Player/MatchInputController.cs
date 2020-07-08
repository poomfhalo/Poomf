using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MatchInputController : MonoBehaviour
{
    public event Action<Vector3> E_OnMoveInput = null;
    public event Action<InputActionPhase> E_OnBallAction = null;
    public event Action E_OnFriendly = null;
    public event Action E_OnEnemy = null;
    public event Action E_OnDodge = null;
    public event Action E_OnFakeFire = null;
    public event Action E_OnJump = null;

    public bool IsEnabled { set; get; } = true;

    void OnDestroy()
    {
        E_OnMoveInput = null;
        E_OnBallAction = null;
        E_OnFriendly = null;
        E_OnEnemy = null;
        E_OnDodge = null;
        E_OnFakeFire = null;
        E_OnJump = null;
    }

    public void I_OnMove(InputAction.CallbackContext ctx)
    {
        if (!IsEnabled)
            return;
        Vector3 v3Input = ctx.ReadValue<Vector2>();
        v3Input.z = v3Input.y;
        v3Input.y = 0;
        E_OnMoveInput?.Invoke(v3Input);
    }
    public void I_OnBallAction(InputAction.CallbackContext ctx)
    {
        if(!IsEnabled)
            return;

        E_OnBallAction?.Invoke(ctx.phase);
    }
    public void I_OnFriendly(InputAction.CallbackContext ctx)
    {
        if (!ButtonDownTest(ctx))
            return;

        E_OnFriendly?.Invoke();
    }
    public void I_OnEnemy(InputAction.CallbackContext ctx)
    {
        if (!ButtonDownTest(ctx))
            return;

        E_OnEnemy?.Invoke();
    }
    public void I_OnDodge(InputAction.CallbackContext ctx)
    {
        if (!ButtonDownTest(ctx))
            return;

        E_OnDodge?.Invoke();
    }
    public void I_OnFakeFire(InputAction.CallbackContext ctx)
    {
        if (!ButtonDownTest(ctx))
            return;

        E_OnFakeFire?.Invoke();
    }
    public void I_OnJump(InputAction.CallbackContext ctx)
    {
        if (!ButtonDownTest(ctx))
            return;

        E_OnJump?.Invoke();
    }
    private bool ButtonDownTest(InputAction.CallbackContext ctx)
    {
        if (!IsEnabled)
            return false;

        if (!ctx.started)
            return false;
        return true;
    }
}