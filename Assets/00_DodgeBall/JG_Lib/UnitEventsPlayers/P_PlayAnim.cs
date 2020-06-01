using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_PlayAnim : UnitPlayable
    {
        enum Mode { Trigger, Bool }
        
        [SerializeField] Mode mode = Mode.Trigger;
        [SerializeField] Animator anim = null;
        [SerializeField] string varToSet = "Work";
        [SerializeField] bool bModeVal = false;
        [SerializeField] float length = 0;

        public override IEnumerator Behavior()
        {
            if (mode == Mode.Trigger)
            {
                anim.SetTrigger(varToSet);
            }
            else if (mode == Mode.Bool)
            {
                anim.SetBool(varToSet, bModeVal);
            }
            yield return new WaitForSeconds(length);
        }
    }
}