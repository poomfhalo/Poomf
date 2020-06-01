using System;
using UnityEngine;
using UnityEngine.Events;

namespace GW_Lib.Utility.Events
{
    [Serializable]
    public class CompoundFloatEvent : CompoundEvent<float>
    {
        [SerializeField] UnityFloatEvent u_FloatEvent = null;
        protected override UnityEvent<float> u_varEvent()
        {
            return u_FloatEvent;
        }
    }
    [Serializable]
    public class CompoundIntEvent : CompoundEvent<int>
    {
        [SerializeField] UnityIntEvent u_IntEvent = null;
        protected override UnityEvent<int> u_varEvent()
        {
            return u_IntEvent;
        }
    }
}