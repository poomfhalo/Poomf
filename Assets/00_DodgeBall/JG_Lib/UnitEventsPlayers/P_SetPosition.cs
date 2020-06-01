using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_SetPosition : UnitPlayable
    {
        [Header("Core")]
        [SerializeField] Transform transformToSet = null;
        [SerializeField] bool setLocal = true;

        [Header("Target Position")]
        [SerializeField] Transform targetPos = null;

        [Header("Offset")]
        [SerializeField] Vector3 targetPosOffset = Vector3.zero;

        public override IEnumerator Behavior()
        {
            if (transformToSet == null)
            {
                yield break;
            }
            if(targetPos)
            {
                if(setLocal)
                {
                    transformToSet.localPosition = targetPos.position + targetPosOffset;
                }
                else
                {
                    transformToSet.position = targetPos.position + targetPosOffset;
                }
            }

            yield break;
        }
    }
}