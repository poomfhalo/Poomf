using System.Collections.Generic;
using UnityEngine;

namespace GW_Lib.Utility
{
    public abstract class GenericGameEvent<ParameterType> : ScriptableObject
    {
        [SerializeField] List<GenericGameEventListener<ParameterType>> listeners = new List<GenericGameEventListener<ParameterType>>();
        [Tooltip("Used For When Raised Is Clicked From Editor, to test the event")]
        [SerializeField] ParameterType sampleParameter = default(ParameterType);
        
        /// <summary>
        /// Used For When Raised Is Clicked From Editor, to test the event
        /// </summary>
        public ParameterType SampleParameter { get { return sampleParameter; } }

        public void Raise(ParameterType parameter)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnGameEventRaised(parameter);
            }
        }
        public void Connect(GenericGameEventListener<ParameterType> listener)
        {
            if (listeners.Contains(listener) == false)
            {
                listeners.Add(listener);
            }
        }
        public void DisConnect(GenericGameEventListener<ParameterType> listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }
    }
}