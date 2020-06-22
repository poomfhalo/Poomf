using System;
using GW_Lib.Utility;
using UnityEngine;

public class BallReciever : DodgeballCharaAction, ICharaAction
{
    public bool IsDetecting => isDetecting;

    public event Action onBallGrabbed = null;

    [SerializeField] TriggerDelegator ballRecieveZone = null;

    [Header("Read Only")]
    [SerializeField] bool isBallIn = false;
    [SerializeField] bool isDetecting = false;
    public bool extCanRecieveBall = true;

    public string actionName => "Recieve Ball";

    void OnEnable()
    {
        GetComponent<CharaHitPoints>().OnHPUpdated += DisableDetection;
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
            {
                SetIsBallIn(true);
                return;
            }
        }
        SetIsBallIn(false);
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
    void FixedUpdate()
    {
        C_RecieveBall();
    }
    public void Cancel()
    {

    }
    public void C_RecieveBall()
    {
        if(extCanRecieveBall && IsDetecting && isBallIn && isButtonClicked)
        {
            RecieveBall();
        }
    }
    public void RecieveBall()
    {
        DodgeballGameManager.instance.OnBallCaught(GetComponent<DodgeballCharacter>());
        GetComponent<BallGrabber>().GrabBall();
        DisableDetection();
        isButtonClicked = false;
        onBallGrabbed?.Invoke();
    }
    public void EnableDetection() => isDetecting = true;
    public void DisableDetection() => isDetecting = false;

    private void SetIsBallIn(bool state)
    {
        if (isBallIn != state)
        {
            isBallIn = state;
            if (state)
            {
                C_RecieveBall();
            }
        }
    }

    #region RecieveButtonClick
    public event Action<bool> onRecievedButtonInput = null;
    public bool isButtonClicked = false;
    public void RecieveButtonInput(bool i)
    {
        isButtonClicked = i;
        onRecievedButtonInput?.Invoke(i);
    }
    #endregion
}