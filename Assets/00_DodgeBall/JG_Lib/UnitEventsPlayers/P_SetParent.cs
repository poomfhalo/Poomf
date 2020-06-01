using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_SetParent : UnitPlayable
    {
        [SerializeField] Transform transformToSet = null;
        [Header("Null for no parent")]
        [SerializeField] Transform newParent = null;
        
        public override IEnumerator Behavior()
        {
            transformToSet.SetParent(newParent);
            yield return 0;
        }
    }
}