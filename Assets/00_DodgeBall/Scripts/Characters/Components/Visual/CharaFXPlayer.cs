using UnityEngine;

public class CharaFXPlayer : MonoBehaviour
{
    [SerializeField] Transform vHitvEffectsHead = null;
    [SerializeField] Transform vDeathEffectsHead = null;
    [SerializeField] Transform vOnThrowStartedOnEnemy = null;
    [SerializeField] Transform vOnThrowStartedOnAlly;

    CharaHitPoints hp = null;
    DodgeballCharacter chara;

    void Awake()
    {
        hp = GetComponent<CharaHitPoints>();
        chara = GetComponent<DodgeballCharacter>();
    }
    void Start()
    {
        hp.OnHpSubtracted += OnHPSubtracted;
        hp.OnZeroHP += OnZeroHP;
        chara.launcher.E_OnThrowStarted += OnThrowStarted;
        chara.launcher.E_OnThrowAnimEnded += OnThrowPointReached;
    }
    void OnDestroy()
    {
        hp.OnHpSubtracted -= OnHPSubtracted;
        hp.OnZeroHP -= OnZeroHP;
        chara.launcher.E_OnThrowStarted -= OnThrowStarted;
        chara.launcher.E_OnThrowAnimEnded -= OnThrowPointReached;
    }

    private void OnHPSubtracted()
    {
        GameExtentions.PlayChildEffect(vHitvEffectsHead);
    }
    private void OnZeroHP()
    {
        GameExtentions.PlayChildEffect(vDeathEffectsHead);
        GameExtentions.PlayChildEffect(vHitvEffectsHead);
    }
    private void OnThrowStarted()
    {
        if (chara.launcher.isLastThrowAtEnemy)
            GameExtentions.PlayChildEffect(vOnThrowStartedOnEnemy);
        else
            GameExtentions.PlayChildEffect(vOnThrowStartedOnAlly);
    }
    private void OnThrowPointReached()
    {
        foreach (Transform t in vOnThrowStartedOnEnemy)
        {
            t.GetComponent<ParticleSystem>()?.Stop(true);
        }
        foreach (Transform t in vOnThrowStartedOnAlly)
        {
            t.GetComponent<ParticleSystem>()?.Stop(true);
        }
    }
}