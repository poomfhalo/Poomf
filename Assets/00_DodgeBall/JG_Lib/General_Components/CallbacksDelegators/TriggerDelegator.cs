using GW_Lib.Utility.Events;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class TriggerDelegator : MonoBehaviour
    {
        public UnityColliderEvent onTriggerEnter;
        public UnityColliderEvent onTriggerExit;
        public Collider GetCollider => GetComponent<Collider>();

        void OnTriggerEnter(Collider other)
        {
            onTriggerEnter?.Invoke(other);
        }
        void OnTriggerExit(Collider other)
        {
            onTriggerExit?.Invoke(other);
        }
    }
}