using System;
using UnityEngine;

public enum DodgeballCommand { GoToChara, LaunchUp, LaunchTo }
public abstract class DodgeballAction : MonoBehaviour, ICharaAction
{
    public abstract DodgeballCommand Command { get; }
    public abstract string actionName { get; }

    public Func<bool> ApplyActionWithCommand = () => true;

    protected Dodgeball ball = null;
    protected ActionsScheduler scheduler = null;
    protected Rigidbody rb3d = null;

    protected virtual void Awake()
    {
        ball = GetComponent<Dodgeball>();
        scheduler = GetComponent<ActionsScheduler>();
        rb3d = GetComponent<Rigidbody>();
    }

    public virtual void Cancel()
    {

    }
}