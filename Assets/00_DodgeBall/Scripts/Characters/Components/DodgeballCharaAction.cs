using System;
using UnityEngine;

[RequireComponent(typeof(ActionsScheduler))]
public class DodgeballCharaAction : MonoBehaviour
{
    public event Action<Vector3> onRecievedInput = null;
    public Vector3 recievedInput = Vector3.zero;

    public virtual void RecieveInput(Vector3 i)
    {
        recievedInput = i;
        onRecievedInput?.Invoke(i);
    }
}