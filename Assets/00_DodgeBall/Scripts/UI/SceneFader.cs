using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public static SceneFader instance
    {
        get
        {
            if (m_instance != null)
                return m_instance;

            m_instance = FindObjectOfType<SceneFader>();
            if (m_instance == null)
            {
                SceneFader prefab = Resources.Load<SceneFader>("Scene Fader");
                m_instance = Instantiate(prefab);
            }
            return m_instance;
        }
    }
    static SceneFader m_instance = null;
    [SerializeField] CanvasGroup group = null;
    [SerializeField] Image cover = null;
    [SerializeField] Color inColor = Color.black;
    [SerializeField] Color outColor = Color.clear;
    [SerializeField] Ease ease = Ease.InOutSine;
    [SerializeField] bool startOut = true;
    [SerializeField] float startOutDur = 1;

    Action lastOnCompleted = null;
    Tween activeTween = null;

    void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        if (m_instance == null)
            m_instance = this;
    }
    void Start()
    {
        group.gameObject.SetActive(true);
        if (startOut)
        {
            FadeOut(startOutDur, null);
        }
    }
    void OnDestroy()
    {
        transform.DOKill();
    }

    public void FadeOut(float dur, Action onCompleted)
    {
        if (activeTween != null)
        {
            activeTween.Kill();
        }
        onCompleted += () =>
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        };
        activeTween = Fade(dur, inColor, outColor, onCompleted);
    }
    public void FadeIn(float dur, Action onCompleted)
    {
        if (activeTween != null)
        {
            activeTween.Kill();
        }
        onCompleted += () =>
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        };
        activeTween = Fade(dur, outColor, inColor, onCompleted);
    }
    private Tween Fade(float dur, Color startColor, Color endColor, Action onCompleted)
    {
        group.interactable = true;
        group.blocksRaycasts = true;

        float lerper = 0;
        return DOTween.To(() => lerper, newF => lerper = newF, 1, dur).SetEase(ease).OnUpdate(() =>
        {
            cover.color = Color.Lerp(startColor, endColor, lerper);
        }).OnComplete(() =>
        {
            onCompleted?.Invoke();
            activeTween = null;
        });
    }
}