using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using GW_Lib;

public class BallShaker : MonoBehaviour
{
    [SerializeField] float shakeDur = 0.15f;
    [SerializeField] int vibrations = 10;
    [SerializeField] Transform target = null;
    [SerializeField] float minSTR = 6;
    [SerializeField] float maxSTR = 16;
    [SerializeField] bool canRun = true;

    Vector3 startLocalPos = Vector3.zero;
    Vector3 startScale = Vector3.zero;
    [Header("Read Only")]
    [SerializeField] Transform oldParent = null;

    Coroutine shakeCoro = null;
    List<Tween> tweens = new List<Tween>();
    void Reset()
    {
        target = transform;
    }
    void Start()
    {
        startLocalPos = target.localPosition;
        startScale = target.localScale;
        oldParent = transform.parent;
    }

    public void ApplyShake(float scaleSTR, float posSTR)
    {
        if (!canRun)
            return;

        scaleSTR = Mathf.Clamp(scaleSTR, minSTR, maxSTR);
        posSTR = Mathf.Clamp(posSTR, minSTR, maxSTR);
        target.transform.SetParent(null);

        //Debug.Break();
        shakeCoro = this.InvokeDelayed(1, () => {
            Tween t = target.DOShakeScale(shakeDur, scaleSTR, vibrations);
            tweens.Add(t);
            t = target.DOShakePosition(shakeDur, posSTR, vibrations).OnComplete(() => {
                target.SetParent(oldParent);
                target.localPosition = startLocalPos;
                target.localScale = startScale;
                tweens.Clear();
            });
            tweens.Add(t);
        });

    }

    public void CancelShake()
    {
        if (!canRun)
            return;

        tweens.ForEach(t => t.Kill());
        tweens.Clear();
        this.KillCoro(ref shakeCoro);
    }
}
