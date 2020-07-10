using System;
using DG.Tweening;
using UnityEngine;

public class ShakeTween : MonoBehaviour
{
    public enum ShakeType { Rot, Pos, Scale }
    [Tooltip("Remember, to change strength when changing this to values appropriate to the type")]
    [SerializeField] ShakeType shakeType = ShakeType.Rot;
    [SerializeField] bool playOnStart = false;
    [SerializeField] Ease ease = Ease.InOutBounce;
    [SerializeField] Transform target = null;
    [SerializeField] float dur = 0.2f;
    [SerializeField] int vibrations = 90;
    [SerializeField] Vector3 strength = new Vector3(0, 0, 90);

    void Reset()
    {
        target = transform;
    }
    void Start()
    {
        if (playOnStart)
            Play(null);
    }

    public void Play(Action onCompleted)
    {
        switch (shakeType)
        {
            case ShakeType.Rot:
                target.DOShakeRotation(dur, strength, vibrations).SetEase(ease).OnComplete(() => {
                    onCompleted?.Invoke();
                });
                break;
            case ShakeType.Pos:
                target.DOShakePosition(dur, strength, vibrations).SetEase(ease).OnComplete(() => {
                    onCompleted?.Invoke();
                });
                break;
            case ShakeType.Scale:
                target.DOShakeScale(dur, strength, vibrations).SetEase(ease).OnComplete(() => {
                    onCompleted?.Invoke();
                });
                break;

        }

    }
}