using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_SetRotation : UnitPlayable
    {
        [SerializeField] Transform transformToSet = null;
        [SerializeField] Vector3 vTargetAngles = new Vector3();
        [SerializeField] Transform targetPos = null;
        [SerializeField] bool setLocal = true;

        public override IEnumerator Behavior()
        {
            if (transformToSet == null)
            {
                yield break;
            }

            if (setLocal)
            {
                transformToSet.localEulerAngles = targetPos != null ? targetPos.localEulerAngles : vTargetAngles;
            }
            else
            {
                transformToSet.eulerAngles = targetPos != null ? targetPos.eulerAngles : vTargetAngles;
            }
        }
    }
}