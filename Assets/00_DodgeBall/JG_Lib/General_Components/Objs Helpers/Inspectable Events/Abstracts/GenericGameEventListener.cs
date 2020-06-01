using UnityEngine;
using UnityEngine.Events;

namespace GW_Lib.Utility
{
    public abstract class GenericGameEventListener<ParameterType> : MonoBehaviour 
    {
        protected abstract UnityEvent<ParameterType> GenericUnityEvent();
        protected abstract GenericGameEvent<ParameterType> GenericGameEvent();

        protected virtual void OnEnable()
        {
            Sub();
        }
        protected virtual void OnDisable()
        {
            UnSub();
        }
        public void OnGameEventRaised(ParameterType parameter)
        {
            GenericUnityEvent().Invoke(parameter);
        }
        public void Sub()
        {
            GenericGameEvent().Connect(this);
        }
        public void UnSub()
        {
            GenericGameEvent().DisConnect(this);
        }
    }
}