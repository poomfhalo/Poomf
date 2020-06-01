using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GW_Lib.Utility
{
    public class P_Emission : UnitPlayable
    {
        [System.Serializable]
        public class TimeToIntensity
        {
            public float time = 0.2f, extraIntensity = 0.5f;
            public int loops = 3;
        }
        public float RestTime => restTime;
        public List<TimeToIntensity> Actions => actions;
        [SerializeField] List<TimeToIntensity> actions = new List<TimeToIntensity>();
        [SerializeField] Ease ease = Ease.InOutSine;
        [SerializeField] LoopType loopType = LoopType.Yoyo;
        [SerializeField] float restTime = 0.2f;
        [SerializeField] TAnimatedEmission emissioner = null;

        [Header("SFX Data")]
        [SerializeField] GOSwitcher sfx = null;

        public override IEnumerator Behavior()
        {
            foreach (TimeToIntensity action in actions)
            {
                bool isDone = false;
                emissioner.AnimateIntensity(action.extraIntensity, action.time, ease, loopType, action.loops, () => {
                    isDone = true;
                });
                if(sfx)
                {
                    sfx.DoSequenceSwitch();
                }
                yield return new WaitUntil(() => isDone);
            }

            emissioner.AnimateIntensity(0, restTime, ease, null);

            yield return new WaitForSeconds(restTime);

        }
    }
}