using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class P_EmissionSingle : UnitPlayable
    {
        [SerializeField] TAnimatedEmission emissioner = null;
        [SerializeField] Gradient colorAnim = null;
        [SerializeField] float time = 0.5f;
        [SerializeField] Ease ease = Ease.InOutSine;
        [SerializeField] float extraIntensity = 0;
        [SerializeField] bool isTint = true;

        public override IEnumerator Behavior()
        {
            bool isDone = false;
            emissioner.AnimateColor(colorAnim, time, ease, isTint, extraIntensity, () =>{
                isDone = true;
            });
            yield return new WaitUntil(() => isDone);
        }
    }
}