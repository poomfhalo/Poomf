using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class GOSwitcher : MonoBehaviour
    {
        [SerializeField] GameObject go = null;
        [SerializeField] bool useFrameDelay = true;
        [SerializeField] int switchFramesDelay = 2;
        [SerializeField] float timeDelay = 1;

        Coroutine delayedSwitch = null;
        
        void Reset()
        {
            go = gameObject;
        }

        public void DoSequenceSwitch()
        {
            if(delayedSwitch != null)
            {
                Transform p = transform.parent.parent;
                if(p!=null)
                {
                    string pn = p.name;
                    Debug.LogWarning("Trying to do, delayed switch very quickly, did not apply switch on " + pn, p);
                }
                else
                {
                    string m = "Trying to do, delayed switch very quickly, did not apply switch on ";
                    Debug.LogWarning(m,gameObject);
                }
                return;
            }

            DoSwitch();
            delayedSwitch = StartCoroutine(DoDelayedSwitch());
        }
        public void DoSwitch()
        {
            go.SetActive(!go.activeSelf);
        }
        private IEnumerator DoDelayedSwitch()
        {
            if (useFrameDelay)
            {
                for (int i = 0; i < switchFramesDelay; i++)
                {
                    yield return 0;
                }
            }
            else
            {
                yield return new WaitForSeconds(timeDelay);
            }
            DoSwitch();
            delayedSwitch = null;
        }
    }
}