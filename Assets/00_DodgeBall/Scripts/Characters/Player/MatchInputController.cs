using System;
using GW_Lib.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

public class MatchInputController : Singleton<MatchInputController>
{
    public static event Action<Vector3> OnMoveInput = null;
    public static event Action OnFire = null;
    public static event Action OnCatch = null;
    public static event Action OnFriendly = null;
    public static event Action OnEnemy = null;
    public static event Action OnDodge = null;
    public static event Action OnFakeFire = null;
    public static event Action OnJump = null;

    public static bool IsEnabled { set; get; } = true;

    void OnDestroy()
    {
        OnMoveInput = null;
        OnFire = null;
        OnCatch = null;
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
    public void I_OnFire(InputAction.CallbackContext ctx)
    {
        if (!ButtonDownTest(ctx))
            return;

        OnFire?.Invoke();
    }
    public void I_OnCatch(InputAction.CallbackContext ctx)
    {
        if (!ButtonDownTest(ctx))
            return;

        OnCatch?.Invoke();
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