using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GW_Lib.Utility
{
    public class P_SpringScale : UnitPlayable
    {
        [SerializeField] MinMaxRange dur = new MinMaxRange(0, 1, 0.25f, 0.5f);
        [SerializeField] Transform target = null;
        [SerializeField] List<Vector3> scalesSequence = new List<Vector3>()
        {
            Vector3.up*0.5f,-Vector3.up*0.5f
        };
        [SerializeField] Ease ease = Ease.InOutSine;

        Vector3 startScale;
        bool isPlaying = false;
        protected override void Reset()
        {
            base.Reset();
            target = transform;
        }

        public override IEnumerator Behavior()
        {
            if(isPlaying)
            {
                yield break;
            }
            isPlaying = true;
            startScale = target.localScale;
            float time = 0;
            foreach (Vector3 scale in scalesSequence)
            {
                Vector3 totalScale = scale + startScale;
                time = dur.GetValue();
                target.DOScale(totalScale, time).SetEase(ease);
                yield return new WaitForSeconds(time);
            }
            time = dur.GetValue();
            target.DOScale(startScale, time).SetEase(ease);
            yield return new WaitForSeconds(time);
            isPlaying = false;
        }
    }
}