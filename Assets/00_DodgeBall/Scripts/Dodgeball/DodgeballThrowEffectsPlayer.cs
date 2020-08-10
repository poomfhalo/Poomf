using UnityEngine;

public class DodgeballThrowEffectsPlayer : MonoBehaviour
{
    [SerializeField] float effectsDeathTime = 2;

    [Header("Read Only")]
    [SerializeField] ParticleSystem currActiveEffect = null;
    [SerializeField] BallThrowData lastUsedData = null;

    DodgeballGoLaunchTo launchTo = null;
    DodgeballThrowSetter throwDataSetter = null;
    Dodgeball ball = null;

    void Awake()
    {
        throwDataSetter = GetComponent<DodgeballThrowSetter>();
        launchTo = GetComponent<DodgeballGoLaunchTo>();
        ball = GetComponent<Dodgeball>();
    }
    void OnEnable()
    {
        launchTo.onLaunchedTo += OnLaunched;
        ball.E_OnStateUpdated += OnStateUpdated;
    }
    void OnDisable()
    {
        launchTo.onLaunchedTo -= OnLaunched;
        ball.E_OnStateUpdated -= OnStateUpdated;
    }

    private void OnLaunched()
    {
        lastUsedData = throwDataSetter.GetLastSelectedThrowData();
        CleanUpCurrEffect();

        if (lastUsedData.throwEffect)
        {
            currActiveEffect = Instantiate(lastUsedData.throwEffect, transform);
            currActiveEffect.transform.localPosition = Vector3.zero;
            currActiveEffect.transform.localRotation = Quaternion.identity;
            currActiveEffect.Play(true);
        }
    }
    private void OnReflected()
    {
        CleanUpCurrEffect();
    }

    private void CleanUpCurrEffect()
    {
        if (currActiveEffect != null)
        {
            currActiveEffect.Stop(true);
            Destroy(currActiveEffect.gameObject, effectsDeathTime);
            currActiveEffect = null;
        }
    }
    private void OnStateUpdated(Dodgeball.BallState newState)
    {
        switch (newState)
        {
            case Dodgeball.BallState.Flying:

                break;
            case Dodgeball.BallState.Held:
                CleanUpCurrEffect();
                break;
            case Dodgeball.BallState.OnGround:
                CleanUpCurrEffect();
                break;
        }
    }
}