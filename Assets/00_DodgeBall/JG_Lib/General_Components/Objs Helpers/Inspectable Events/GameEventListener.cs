using UnityEngine;
using UnityEngine.Events;

namespace GW_Lib.Utility
{
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] GameEvent eventToListenTo=null;
        public UnityEvent unityEvent;

        public void OnGameEventRaised()
        {
            unityEvent.Invoke();
        }
        public void SubToEvent()
        {
            eventToListenTo.Sub(this);
        }
        public void UnSubFromEvent()
        {
            eventToListenTo.UnSub(this);
        }
    }
}