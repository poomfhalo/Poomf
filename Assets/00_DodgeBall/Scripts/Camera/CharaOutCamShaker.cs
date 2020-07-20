using UnityEngine;
using Cinemachine;
using DG.Tweening;
using GW_Lib;

public class CharaOutCamShaker : MonoBehaviour
{
    [SerializeField] float shakeAmp = 3;
    [SerializeField] float shakeFreq = 3;
    [SerializeField] float shakeDur = 0.3f;
    [SerializeField] float shakeEaseInDur = 0.05f;
    [SerializeField] float shakeEaseOutDur = 0.05f;
    [Tooltip("Activates the moment, the character is teleported to the appropriate position")]
    [SerializeField] bool shakeOnTeleportedOut = true;
    [Tooltip("Activates the moment, the character turns into a rag doll/has zero hp")]
    [SerializeField] bool shakeOnKnockedOut = true;

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
        });
    }

    private void OnCharaKnockedOut(DodgeballCharacter chara)
    {
        if (isShaking)
            return;

        isShaking = true;
        EaseIntoShake();
        this.InvokeDelayed(shakeDur + shakeEaseInDur, EaseOutOfShake);

        void EaseIntoShake()
        {
            float lerper = 0;
            float startAmp = noise.m_AmplitudeGain;
            float startFreq = noise.m_FrequencyGain;

            DOTween.To(() => lerper, f => lerper = f, 1, shakeEaseInDur).SetEase(Ease.InOutSine).OnUpdate(() =>{
                noise.m_AmplitudeGain = Mathf.Lerp(startAmp, shakeAmp, lerper);
                noise.m_FrequencyGain = Mathf.Lerp(startFreq, shakeFreq, lerper);
            }); ;
        }
        void EaseOutOfShake()
        {
            float lerper = 0;
            float startAmp = noise.m_AmplitudeGain;
            float startFreq = noise.m_FrequencyGain;

            DOTween.To(() => lerper, f => lerper = f, 1, shakeEaseOutDur).SetEase(Ease.InOutSine).OnUpdate(()=> {
                noise.m_AmplitudeGain = Mathf.Lerp(startAmp, 0, lerper);
                noise.m_FrequencyGain = Mathf.Lerp(startFreq, 0, lerper);
            }).OnComplete(()=> {
                isShaking = false;
            });
        }
    }
}