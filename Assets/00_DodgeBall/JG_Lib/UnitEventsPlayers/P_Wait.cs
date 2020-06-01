using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_Wait : UnitPlayable
    {
        [SerializeField] float duration = 0.1f;
        public override IEnumerator Behavior()
        {
            yield return new WaitForSeconds(duration);
        }
    }
}