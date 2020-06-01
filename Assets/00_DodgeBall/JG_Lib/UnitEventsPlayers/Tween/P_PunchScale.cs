using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_PunchScale : UnitPlayable
    {
        [SerializeField] float minDur = 0.1f;
        [SerializeField] float maxDur = 0.5f;
        [SerializeField] Vector3 byMin = new Vector3(-0.1f, -0.1f, 0);
        [SerializeField] Vector3 byMax = new Vector3(0.1f, 0.1f, 0);
        [Tooltip("use -1 for infinite looping")]
        [SerializeField] int loops = -1;
        [SerializeField] LoopType loopType = LoopType.Yoyo;
        [SerializeField] bool playOnEnable = false;
        [SerializeField] bool wait = false;

        Vector3 startScale;

        public override IEnumerator Behavior()
        {
            startScale = transform.localScale;
            float dur = GetDur();
            transform.DOScale(GetBy() + startScale, dur).SetLoops(loops, loopType);
            if(wait)
            {
                yield return new WaitForSeconds(dur);
            }
        }
        void OnEnable()
        {
            if (playOnEnable)
            {
                Behavior();
            }
        }
        private Vector3 GetBy()
        {
            return Extentions.RandomRange(byMin, byMax);
        }
        private float GetDur()
        {
            return UnityEngine.Random.Range(minDur, maxDur);
        }
    }
}