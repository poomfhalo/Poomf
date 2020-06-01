using System;
using UnityEngine;
using UnityEngine.Events;

namespace GW_Lib.Utility.Events
{
    [Serializable]
    public class CompoundEvent
    {
        public event Action codeEvent;
        [SerializeField] UnityEvent u_Event = null;
        public void CallEvent()
        {
            codeEvent?.Invoke();
            u_Event.Invoke();
        }
        public void Connect(Action action)
        {
            codeEvent += action;
        }
        public void DisConnect(Action action)
        {
            codeEvent -= action;
        }

        public static CompoundEvent operator +(CompoundEvent self, Action action)
        {
            self.Connect(action);
            return self;
        }
        public static CompoundEvent operator -(CompoundEvent self, Action action)
        {
            self.DisConnect(action);
            return self;
        }
    }
}