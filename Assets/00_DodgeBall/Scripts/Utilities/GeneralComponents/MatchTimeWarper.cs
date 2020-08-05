using System;
using DG.Tweening;
using GW_Lib;
using UnityEngine;

public class MatchTimeWarper : MonoBehaviour
{
    [SerializeField] float slowingEaseInDur = 0.3f;
    [SerializeField] float slowingEaseOutDur = 0.3f;

    [Range(0.01f,1)]
    [SerializeField] float timeReductionPercent = 0.5f;

    float remainingTimeFromSlow => 1 - timeReductionPercent;

    bool isSlowingDown = false;
    void OnDestroy()
    {
        Time.timeScale = 1;
        transform.DOKill();
    }

    public void SlowTime(float dur,Action onCompleted)
    {
        if (isSlowingDown)
            return;

        isSlowingDown = true;


        EaseInToSlow();
        this.InvokeDelayed(slowingEaseInDur + dur, EaseOutOfSlow);
        this.InvokeDelayed(slowingEaseInDur + dur + slowingEaseOutDur + 0.01f, onCompleted);
    }

    private void EaseInToSlow()
    {
        float lerper = 0;
        DOTween.To(() => lerper, f => lerper = f, 1, slowingEaseInDur).SetEase(Ease.InOutSine).OnUpdate(() =>
        {
            Time.timeScale = Mathf.Lerp(1, remainingTimeFromSlow, lerper);
        }).SetUpdate(true);
    }
    private void EaseOutOfSlow()
    {
        float lerper = 0;
        DOTween.To(() => lerper, f => lerper = f, 1, slowingEaseOutDur).SetEase(Ease.InOutSine).OnUpdate(() =>
        {
            Time.timeScale = Mathf.Lerp(remainingTimeFromSlow, 1, lerper);
        }).SetUpdate(true);
    }
}