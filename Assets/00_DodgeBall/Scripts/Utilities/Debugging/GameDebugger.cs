using System.Collections.Generic;
using GW_Lib.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameDebugger : Singleton<GameDebugger>
{
    public InputAction enableDebugging = null;
    public InputAction disableDebugging = null;

    [Header("Read Only")]
    public List<GameDebuggable> debuggables = new List<GameDebuggable>();
    [SerializeField] bool lastState = true;

    public static void AddDebuggable(GameDebuggable debuggable)
    {
        if (!instance)
            return;
        if (instance.debuggables.Contains(debuggable))
            return;

        if (Debug.isDebugBuild)
        {
            instance.debuggables.Add(debuggable);
        }
        else
        {
            debuggable.SetActivity(false);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (Debug.isDebugBuild || Application.isEditor)
        {
            enableDebugging.performed += EnableDebugging;
            disableDebugging.performed += DisableDebugging;
            enableDebugging.Enable();
            disableDebugging.Enable();
        }
    }

    private void EnableDebugging(InputAction.CallbackContext ctx)
    {
        debuggables.RemoveAll(d => d == null);
        foreach (var d in debuggables)
        {
            d.SetActivity(true);
        }
        lastState = true;
    }
    private void DisableDebugging(InputAction.CallbackContext ctx)
    {
        debuggables.RemoveAll(d => d == null);
        foreach (var d in debuggables)
        {
            d.SetActivity(false);
        }
        lastState = false;
    }
}