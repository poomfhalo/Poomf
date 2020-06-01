using System;
using UnityEngine.Events;

namespace GW_Lib.Utility.Events
{
    public abstract class CompoundEvent<T>
    {
        public event Action<T> varCodeEvent;
        protected abstract UnityEvent<T> u_varEvent();

        public void CallEvent(T var)
        {
            varCodeEvent?.Invoke(var);
            u_varEvent().Invoke(var);
        }

        public void Connect(Action<T> action)
        {
            varCodeEvent += action;
        }
        public void DisConnect(Action<T> action)
        {
            varCodeEvent -= action;
        }

        public static CompoundEvent<T> operator +(CompoundEvent<T> self, Action<T> action)
        {
            self.Connect(action);
            return self;
        }
        public static CompoundEvent<T> operator -(CompoundEvent<T> self, Action<T> action)
        {
            self.DisConnect(action);
            return self;
        }
    }
}