using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_Debug : UnitPlayable
    {
        enum MessageType{Log,Warning,Error}
        [SerializeField] MessageType messageType = MessageType.Log;
        [SerializeField] string message = "";
        public override IEnumerator Behavior()
        {
            DoDebug();
            yield break;
        }

        public void DoDebug()
        {
            switch(messageType)
            {
                case MessageType.Log:
                    Debug.Log(message,gameObject);
                    break;
                case MessageType.Warning:
                    Debug.LogWarning(message,gameObject);
                    break;
                case MessageType.Error:
                    Debug.LogError(message,gameObject);
                    break;
            }
        }
    }
}