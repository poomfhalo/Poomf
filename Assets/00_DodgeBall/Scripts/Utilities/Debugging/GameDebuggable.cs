using UnityEngine;

public abstract class GameDebuggable : MonoBehaviour
{
    protected virtual void Awake()
    {
        GameDebugger.AddDebuggable(this);
    }
    public abstract void SetActivity(bool toState);
}
