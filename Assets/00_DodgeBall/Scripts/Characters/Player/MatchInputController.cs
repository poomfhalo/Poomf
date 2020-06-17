using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MatchInputController : MonoBehaviour
{
    public event Action<Vector3> OnMoveInput = null;
    public event Action<InputActionPhase> OnBallAction = null;
    public event Action OnFriendly = null;
    public event Action OnEnemy = null;
    public event Action OnDodge = null;
    public event Action OnFakeFire = null;
    public event Action OnJump = null;

    public bool IsEnabled { set; get; } = true;

    void OnDestroy()
    {
        OnMoveInput = null;
        OnBallAction = null;
        OnFriendly = null;
        OnEnemy = null;
        OnDodge = null;
        OnFakeFire = null;
        OnJump = null;
    }
    public void I_OnMove(InputAction.CallbackContext ctx)
    {
        if (!IsEnabled)
            return;

        Vector3 v3Input = ctx.ReadValue<Vector2>();
        v3Input.z = v3Input.y;
        v3Input.y = 0;
        OnMoveInput?.Invoke(v3Input);
    }
    public void I_OnBallAction(InputAction.CallbackContext ctx)
    {
        if(!IsEnabled)
            return;

        OnBallAction?.Invoke(ctx.phase);
    }
    public void I_OnFriendly(InputAction.CallbackContext ctx)
    {
        if (!ButtonDownTest(ctx))
            return;

        OnFriendly?.Invoke();
    }
    public void I_OnEnemy(InputAction.CallbackContext ctx)
    {
        if (!ButtonDownTest(ctx))
            return;

        OnEnemy?.Invoke();
    }
    public void I_OnDodge(InputAction.CallbackContext ctx)
    {
        if (!ButtonDownTest(ctx))
            return;

        OnDodge?.Invoke();
    }
    public void I_OnFakeFire(InputAction.CallbackContext ctx)
    {
        if (!ButtonDownTest(ctx))
            return;

        OnFakeFire?.Invoke();
    }
    public void I_OnJump(InputAction.CallbackContext ctx)
    {
        if (!ButtonDownTest(ctx))
            return;

        OnJump?.Invoke();
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