using GW_Lib.Utility.Events;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollisionDelegator : MonoBehaviour
{
    public Collider GetCollider => GetComponent<Collider>();
    public UnityCollisionEvent onCollisionEnter = null;
    public UnityCollisionEvent onCollisionExit = null;

    void OnCollisionEnter(Collision col)
    {
        onCollisionEnter?.Invoke(col);
    }
    void OnCollisionExit(Collision col)
    {
        onCollisionExit?.Invoke(col);
    }
}
