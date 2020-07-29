using UnityEngine;
using Cinemachine;
using DG.Tweening;
using GW_Lib;

public class CharaOutCamShaker : MonoBehaviour
{
    [Header("CoreData")]
    [SerializeField] float shakeDur = 0.3f;
    [SerializeField] float shakeEaseInDur = 0.05f;
    [SerializeField] float shakeEaseOutDur = 0.05f;
    [Tooltip("Activates the moment, the character is teleported to the appropriate position")]
    [SerializeField] bool shakeOnTeleportedOut = true;
    [Tooltip("Activates the moment, the character turns into a rag doll/has zero hp")]
    [SerializeField] bool shakeOnKnockedOut = true;

    [Header("Konckout Shake Data")]
    [SerializeField] float shakeAmp = 3;
    [SerializeField] float shakeFreq = 3;
    [Header("Hit Shake Data")]
    [SerializeField] float hitShakeAmp = 1.5f;
    [SerializeField] float hitShakeFreq = 1.5f;

    CinemachineVirtualCamera cam = null;
    CinemachineBasicMultiChannelPerlin noise = null;
    bool isShaking = false;

    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
        GameIntroManager.instance.OnEntryCompleted += OnEntryCompleted;
    }
    void OnDestroy()
    {
        if(TeamsManager.instance)
        {
            TeamsManager.instance.AllCharacters.ForEach(c =>{
                if (c)
                {
                    CharaKnockoutPlayer knockedOut = c.GetComponent<CharaKnockoutPlayer>();
                    if(knockedOut)
                    {
                        knockedOut.E_OnTeleportedOut -= OnCharaKnockedOut;
                        knockedOut.E_OnKnockedOut -= OnCharaKnockedOut;
                    }
                    CharaHitPoints hp = c.GetComponent<CharaHitPoints>();
                    if (hp)
                        hp.OnHpSubtracted -= OnHpSubtracted;
                }
            });
        }
        if (GameIntroManager.instance)
            GameIntroManager.instance.OnEntryCompleted -= OnEntryCompleted;
    }

    private void OnEntryCompleted()
    {
        TeamsManager.instance.AllCharacters.ForEach(c => {
            CharaKnockoutPlayer knockedOut = c.GetComponent<CharaKnockoutPlayer>();
            knockedOut.E_OnTeleportedOut += OnCharaKnockedOut;
            knockedOut.E_OnKnockedOut += OnCharaKnockedOut;
            CharaHitPoints hp = c.GetComponent<CharaHitPoints>();
            hp.OnHpSubtracted += OnHpSubtracted;
        });
    }
    private void OnHpSubtracted()
    {
        ApplyShake(hitShakeAmp, hitShakeFreq);
    }
    private void OnCharaKnockedOut(DodgeballCharacter chara)
    {
        ApplyShake(shakeAmp, shakeFreq);
    }

    private void ApplyShake(float amp, float freq)
    {
        if (isShaking)
            return;

        isShaking = true;
        EaseIntoShake(amp, freq);
        this.InvokeDelayed(shakeDur + shakeEaseInDur, EaseOutOfShake);
    }
    void EaseIntoShake(float amp,float freq)
    {
        float lerper = 0;
        float startAmp = noise.m_AmplitudeGain;
        float startFreq = noise.m_FrequencyGain;

        DOTween.To(() => lerper, f => lerper = f, 1, shakeEaseInDur).SetEase(Ease.InOutSine).OnUpdate(() => {
            noise.m_AmplitudeGain = Mathf.Lerp(startAmp, amp, lerper);
            noise.m_FrequencyGain = Mathf.Lerp(startFreq, freq, lerper);
        });
    }
    void EaseOutOfShake()
    {
        float lerper = 0;
        float startAmp = noise.m_AmplitudeGain;
        float startFreq = noise.m_FrequencyGain;

        DOTween.To(() => lerper, f => lerper = f, 1, shakeEaseOutDur).SetEase(Ease.InOutSine).OnUpdate(() => {
            noise.m_AmplitudeGain = Mathf.Lerp(startAmp, 0, lerper);
            noise.m_FrequencyGain = Mathf.Lerp(startFreq, 0, lerper);
        }).OnComplete(() => {
            isShaking = false;
        });
    }
}