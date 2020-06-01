using GW_Lib.Utility;
using UnityEngine;

public class PC : DodgeballCharacter
{
    [Header("Ball Grabbing")]
    [SerializeField] TriggerDelegator ballGrabbingZone = null;

    [Header("Read Only")]
    [SerializeField] bool isBallInGrabZone = false;

    protected override void Awake()
    {
        base.Awake();
        ballGrabbingZone.onTriggerEnter.AddListener(OnBallGrabZoneEntered);
        ballGrabbingZone.onTriggerExit.AddListener(OnBallGrabZoneExitted);
    }
    void OnEnable()
    {
        MatchInputController.OnMoveInput += OnMoveInput;
        MatchInputController.OnCatch += OnCatch;
        MatchInputController.OnEnemy += OnEnemy;
        MatchInputController.OnFriendly += OnFriendly;
        MatchInputController.OnFire += OnFire;
        MatchInputController.OnDodge+= OnDodge;
    }
    void OnDisable()
    {
        MatchInputController.OnMoveInput -= OnMoveInput;
        MatchInputController.OnCatch -= OnCatch;
        MatchInputController.OnEnemy -= OnEnemy;
        MatchInputController.OnFriendly -= OnFriendly;
        MatchInputController.OnFire -= OnFire;
        MatchInputController.OnDodge -= OnDodge;
    }

    private void OnMoveInput(Vector3 i)
    {
        if (launcher.IsThrowing)
            return;
        mover.StartMoveByInput(i, cam.transform);
    }
    private void OnCatch()
    {
        if (isBallInGrabZone)
            GrabBall();
    }

    private void OnBallGrabZoneEntered(Collider col)
    {
        if (!col.GetComponent<Dodgeball>())
            return;
        isBallInGrabZone = true;
    }
    private void OnBallGrabZoneExitted(Collider col)
    {
        if (!col.GetComponent<Dodgeball>())
            return;

        isBallInGrabZone = false;
    }

    private void OnFriendly()
    {
        if(HasBall)
        {
            selectionIndicator.SetNewFocus(true);
        }
    }
    private void OnEnemy()
    {
        if (HasBall)
        {
            selectionIndicator.SetNewFocus(false);
        }
    }
    private void OnFire()
    {
        if (IsThrowing)
            return;
        if(HasBall)
        {
            launcher.StartThrowAction(selectionIndicator.ActiveSelection);
        }
    }
    private void OnDodge()
    {
        if (HasBall)
            return;
        if (IsThrowing)
            return;
        if (IsDodging)
            return;

        dodger.StartDodgeAction();
    }
}