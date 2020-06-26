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
    CharaHitPoints hp => GetComponent<CharaHitPoints>();

    void OnEnable() => hp.OnHPUpdated += DisableDetection;
    void OnDisable() => hp.OnHPUpdated -= DisableDetection;

    #region BallDetection
    void Update()
    {
        if(!IsDetecting)
        {
            SetIsBallIn(false);
            return;
        }
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
    #endregion
    void FixedUpdate()
    {
        C_RecieveBall();
    }
    public void Cancel() { }

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
        isBallIn = state;
        if(isBallIn)
            C_RecieveBall();
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