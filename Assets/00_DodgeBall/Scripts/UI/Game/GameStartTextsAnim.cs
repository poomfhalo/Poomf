using System;
using DG.Tweening;
using UnityEngine;

public class GameStartTextsAnim : MonoBehaviour
{
    [SerializeField] Ease ease = Ease.InOutCirc;
    [SerializeField] Ease dodgeEase = Ease.InOutBounce;
    [SerializeField] InOutTween oneTween = null;
    [SerializeField] InOutTween twoTween = null;
    [SerializeField] InOutTween threeTween = null;
    [SerializeField] InOutTween dodgeTween = null;

    [SerializeField] bool playOnStart = false;

    void Start()
    {
        if(playOnStart)
        {
            Play(null);
        }
    }

    public void Play(Action onCompleted)
    {
        oneTween.Play(null, () =>{
            twoTween.Play(null, () =>{
                threeTween.Play(null, () =>{
                    Action onOut = () => onCompleted?.Invoke();
                    dodgeTween.Play(null,null,onOut);
                }, null);
            }, null);
        }, null);
    }
}