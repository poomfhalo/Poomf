using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GW_Lib.Utility
{
    public class P_CallFunction : UnitPlayable
    {
        [SerializeField] float delayBef = 0;
        [SerializeField] float delayAfter = 0.1f;
        [SerializeField] UnityEvent u_Function = null;

        public override IEnumerator Behavior()
        {
            if (delayBef > 0)
            {
                yield return new WaitForSeconds(delayBef);
            }
            u_Function.Invoke();
            if (delayAfter > 0)
            {
                yield return new WaitForSeconds(delayAfter);
            }
        }
    }
}