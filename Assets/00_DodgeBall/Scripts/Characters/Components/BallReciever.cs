using System;
using GW_Lib.Utility;
using UnityEngine;

public class BallReciever : DodgeballCharaAction, ICharaAction
{
    public event Action onBallGrabbed = null;
    public Func<bool> ExtCanDetectBall = () => true;

    [SerializeField] TriggerDelegator ballRecieveZone = null;

    [Header("Read Only")]
    [SerializeField] bool isBallIn = false;

    public string actionName => "Recieve Ball";

    void OnEnable()
    {
        GetComponent<CharaHitPoints>().OnHPUpdated += OnHPUpdated;
        ballRecieveZone.onTriggerEnter.AddListener(OnEntered);
        ballRecieveZone.onTriggerExit.AddListener(OnExitted);
    }
    void OnDisable()
    {
        ballRecieveZone.onTriggerEnter.RemoveListener(OnEntered);
        ballRecieveZone.onTriggerExit.RemoveListener(OnExitted);
    }

    #region BallDetection
    void Update()
    {
        Bounds b = ballRecieveZone.GetCollider.bounds;
        Collider[] overlaps = Physics.OverlapBox(b.center, b.extents);
        foreach (var col in overlaps)
        {
            if (col.GetComponent<Dodgeball>())
                SetIsBallIn(true);
        }
    }
    private void OnEntered(Collider other)
    {
        if (!other.GetComponent<Dodgeball>())
            return;

        SetIsBallIn(true);
    }
    private void OnExitted(Collider other)
    {
        if (!other.GetComponent<Dodgeball>())
            return;

        SetIsBallIn(false);
    }
    #endregion

    private void OnHPUpdated()
    {

    }

    public void Cancel()
    {

    }
    public void TryGrabBall()
    {
        if(isBallIn && isButtonClicked)
        {
            DodgeballGameManager.instance.OnBallCaught(GetComponent<DodgeballCharacter>());
            GetComponent<BallGrabber>().GrabBall();
            isButtonClicked = false;
            onBallGrabbed?.Invoke();
        }
    }

    private void SetIsBallIn(bool state)
    {
        if (isBallIn != state)
        {
            isBallIn = state;
            if (state)
            {
                TryGrabBall();
            }
        }
    }
}