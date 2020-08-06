using GW_Lib;
using UnityEngine;

public class CharaFXPlayer : MonoBehaviour
{
    [SerializeField] float timeBeforeFirstPlay = 0.5f;

    [SerializeField] Transform vHitvEffectsHead = null;
    [SerializeField] Transform vDeathEffectsHead = null;
    [SerializeField] Transform vOnThrowStartedOnEnemy = null;
    [SerializeField] Transform vOnThrowStartedOnAlly = null;
    [SerializeField] Transform vOnJump = null;
    [SerializeField] Transform vOnLand = null;
    [SerializeField] Transform vOnStep = null;
    [Header("Body Parts")]
    [SerializeField] Transform leftFoot = null;
    [SerializeField] Transform rightFoot = null;

    CharaHitPoints hp = null;
    DodgeballCharacter chara = null;
    Jumper jumper = null;
    bool hasTimePassed = false;

    void Awake()
    {
        hp = GetComponent<CharaHitPoints>();
        chara = GetComponent<DodgeballCharacter>();
        jumper = GetComponent<Jumper>();
    }
    void OnEnable()
    {
        this.InvokeDelayed(timeBeforeFirstPlay, () => hasTimePassed = true);
    }
    void OnDisable()
    {
        hasTimePassed = false;
    }
    void Start()
    {
        hp.OnHpSubtracted += OnHPSubtracted;
        hp.OnZeroHP += OnZeroHP;
        chara.launcher.E_OnThrowStarted += OnThrowStarted;
        chara.launcher.E_OnThrowAnimEnded += OnThrowPointReached;
        jumper.E_OnJumped += OnJumped;
        jumper.E_OnLanded += OnLanded;
    }
    void OnDestroy()
    {
        hp.OnHpSubtracted -= OnHPSubtracted;
        hp.OnZeroHP -= OnZeroHP;
        chara.launcher.E_OnThrowStarted -= OnThrowStarted;
        chara.launcher.E_OnThrowAnimEnded -= OnThrowPointReached;
        jumper.E_OnJumped -= OnJumped;
        jumper.E_OnLanded -= OnLanded;
    }

    private void OnJumped()
    {
        PlayVFX(vOnJump, true, 5);
    }
    private void OnLanded()
    {
        PlayVFX(vOnLand);
    }
    private void OnHPSubtracted()
    {
        PlayVFX(vHitvEffectsHead);
    }
    private void OnZeroHP()
    {
        PlayVFX(vDeathEffectsHead);
        PlayVFX(vHitvEffectsHead);
    }
    private void OnThrowStarted()
    {
        if (chara.launcher.isLastThrowAtEnemy)
            PlayVFX(vOnThrowStartedOnEnemy);
        else
            PlayVFX(vOnThrowStartedOnAlly);
    }
    private void OnThrowPointReached()
    {
        StopVFX(vOnThrowStartedOnEnemy);
        StopVFX(vOnThrowStartedOnAlly);
    }
    private Transform PlayVFX(Transform head, bool makeCopy = false, float copyLifeTime = -1)
    {
        if (!hasTimePassed || head == null)
            return null;
        if (makeCopy && copyLifeTime > 0)
            return GameExtentions.PlayChildEffectMake(head, copyLifeTime);
        else
            return GameExtentions.PlayChildEffect(head);
    }
    private void StopVFX(Transform head)
    {
        if (!hasTimePassed || head == null)
            return;

        foreach (Transform t in head)
        {
            t.GetComponent<ParticleSystem>()?.Stop(true);
        }
    }

    public void A_LeftStep()
    {
        Transform fx = PlayVFX(vOnStep, true, 5);
        fx.transform.position = leftFoot.position;
    }
    public void A_RightStep()
    {
        Transform fx = PlayVFX(vOnStep, true, 5);
        fx.transform.position = rightFoot.position;
    }
}