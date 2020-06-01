using System.Collections.Generic;
using UnityEngine;

namespace GW_Lib.Utility
{
    [CreateAssetMenu(fileName = "Game Event", menuName ="GW_Lib/Game Events/New Game Event")]
    public class GameEvent : ScriptableObject
    {
        [SerializeField] List<GameEventListener> listeners = new List<GameEventListener>();
        public void Sub(GameEventListener listener)
        {
            if (listeners.Contains(listener)==false)
            {
                listeners.Add(listener);
            }
        }
        public void UnSub(GameEventListener listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }
        public void Raise()
        {
            for (int i = listeners.Count-1; i >= 0; i--)
            {
                listeners[i].OnGameEventRaised();
            }
        }

        private void ReSet()
        {
            listeners.Clear();
        }
        private void OnEnable()
        {
            ReSet();
        }
        private void OnDisable()
        {
            ReSet();
        }
    }
}