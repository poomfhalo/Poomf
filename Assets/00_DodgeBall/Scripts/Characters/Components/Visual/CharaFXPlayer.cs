using UnityEngine;

public class CharaFXPlayer : MonoBehaviour
{
    [SerializeField] Transform vHitvEffectsHead = null;
    [SerializeField] Transform vDeathEffectsHead = null;
    [SerializeField] Transform vOnThrowStarted = null;

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
        chara.launcher.E_OnThrowPointReached += OnThrowPointReached;
    }
    void OnDestroy()
    {
        hp.OnHpSubtracted -= OnHPSubtracted;
        hp.OnZeroHP -= OnZeroHP;
        chara.launcher.E_OnThrowStarted -= OnThrowStarted;
        chara.launcher.E_OnThrowPointReached -= OnThrowPointReached;
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
        GameExtentions.PlayChildEffect(vOnThrowStarted);
    }
    private void OnThrowPointReached()
    {
        foreach (Transform t in vOnThrowStarted)
        {
            t.GetComponent<ParticleSystem>()?.Stop(true);
        }
    }
}