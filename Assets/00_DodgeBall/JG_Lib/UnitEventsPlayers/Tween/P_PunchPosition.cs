using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_PunchPosition : UnitPlayable
    {
        [SerializeField] Transform target = null;
        [SerializeField] Vector3 minPunch = -Vector3.one;
        [SerializeField] Vector3 maxPunch = Vector3.one;
        [SerializeField] float mag = 0.1f;
        [SerializeField] float dur = 1;
        [SerializeField] int vibrations = 10;
        [Range(0, 1)]
        [SerializeField] float elasticity = 0.75f;
        [SerializeField] bool allowWaitInSequence = false;

        public override IEnumerator Behavior()
        {
            DoPunch();
            if (allowWaitInSequence)
            {
                yield return new WaitForSeconds(dur);
            }
        }
        public void DoPunch()
        {
            Vector3 rndPunch = Extentions.RandomRange(minPunch, maxPunch).normalized;
            if (target == null)
            {
                target = transform;
            }
            target.DOPunchPosition(rndPunch * mag, dur, vibrations, elasticity);
        }
    }
}