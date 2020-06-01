using UnityEngine;
using DG.Tweening;

namespace GW_Lib.Utility
{
    public class Spring : MonoBehaviour
    {
        [SerializeField] Vector3 highest = new Vector3(0, 0.1f, 0);
        [SerializeField] Vector3 lowest = new Vector3(0, -0.1f, 0);
        [SerializeField] MinMaxRange dur = new MinMaxRange(0, 1, 0.75f, 1);
        [SerializeField] Ease ease = Ease.InOutFlash;
        [SerializeField] Transform target = null;

        [Header("Read Only")]
#pragma warning disable RECS0122 // Initializing field with default value is redundant
        [SerializeField] bool isRising = false;
#pragma warning restore RECS0122 // Initializing field with default value is redundant
        [SerializeField] Vector3 startPos = Vector3.zero;

        void Reset()
        {
            target = transform;
        }
        void Awake()
        {
            startPos = target.localPosition;
        }
        void OnDisable()
        {
            transform.DOKill();
        }
        void OnEnable()
        {
            isRising = true;
            GoUp();
        }

        private void GoUp()
        {
            target.DOLocalMove(highest + startPos, dur.GetValue()).SetEase(ease).OnStepComplete(() =>
            {
                isRising = false;
                GoDown();
            });
        }

        private void GoDown()
        {
            target.DOLocalMove(lowest + startPos, dur.GetValue()).SetEase(ease).OnStepComplete(() =>
            {
                isRising = true;
                GoUp();
            });
        }
    }
}