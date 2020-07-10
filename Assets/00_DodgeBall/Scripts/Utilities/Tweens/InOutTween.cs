using System;
using UnityEngine;
using DG.Tweening;
using GW_Lib;

public class InOutTween : MonoBehaviour
{
    public enum ActOn { Scale,Position }
    [Tooltip("Remember, to change outVal and inVal when changing this to values appropriate to the type")]
    [SerializeField] ActOn actsOn = ActOn.Scale;
    [SerializeField] Vector3 outVal = Vector3.zero;
    [SerializeField] Vector3 inVal = Vector3.one;
    [SerializeField] Transform target = null;
    [SerializeField] float inDur = 0.4f;
    [SerializeField] float delay = 0.2f;
    [SerializeField] float outDur = 0.5f;
    [SerializeField] Ease ease = Ease.InOutSine;
    [SerializeField] bool startOut = true;

    ShakeTween shakeTween => GetComponent<ShakeTween>();
    RectTransform rTarget => target.GetComponent<RectTransform>();

    void Reset()
    {
        target = transform;
    }
    void Start()
    {
        if(startOut)
        {
            switch (actsOn)
            {
                case ActOn.Position:
                    target.position = outVal;
                    break;
                case ActOn.Scale:
                    target.localScale = outVal;
                    break;
            }
        }
    }
    void OnDestroy()
    {
        target.DOKill();
    }

    public void Play(Action onIn,Action onDelayEnded,Action onOut)
    {
        if(shakeTween)
        {
            onIn += () => shakeTween.Play(null);
        }
        onIn += () => shakeTween?.Play(null);
        onIn += () => {
            this.InvokeDelayed(delay, () =>
            {
                onDelayEnded?.Invoke();
                switch (actsOn)
                {
                    case ActOn.Scale:
                        target.DOScale(outVal, outDur).SetEase(ease).OnComplete(() =>
                        {
                            onOut?.Invoke();
                        });
                        break;
                    case ActOn.Position:
                        DoUIMove(outVal, outDur, onOut);
                        break;
                }

            });
        };

        switch (actsOn)
        {
            case ActOn.Position:
                DoUIMove(inVal, inDur, onIn);
                break;
            case ActOn.Scale:
                target.DOScale(inVal, inDur).SetEase(ease).OnComplete(() => onIn?.Invoke()); 
                break;
        }
    }

    private void DoUIMove(Vector3 to,float dur, Action onCompleted)
    {
        float lerper = 0;
        Vector3 startPos = rTarget.anchoredPosition;
        DOTween.To(() => lerper, newF => lerper = newF, 1, dur).SetEase(ease).OnUpdate(() =>{
            rTarget.anchoredPosition = Vector3.Slerp(startPos, to, lerper);
        }).OnComplete(() => onCompleted?.Invoke());
    }
}