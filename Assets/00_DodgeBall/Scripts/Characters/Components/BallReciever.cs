﻿using System;
using GW_Lib.Utility;
using UnityEngine;

public class BallReciever : DodgeballCharaAction, ICharaAction, IEnergyAction
{
    public bool IsDetecting => isDetecting;
    public bool CanRecieveBallNow => extCanRecieveBall && IsDetecting && isBallIn && isButtonClicked;
    public event Action onBallGrabbed = null;

    [SerializeField] TriggerDelegator ballRecieveZone = null;
    [SerializeField] float timeToConsiderAsNewCatch = 0.1f;
    [SerializeField] float energyCostPerSec = 40;
    [Header("Read Only")]
    [SerializeField] bool isBallIn = false;
    [SerializeField] bool isDetecting = false;
    public bool extCanRecieveBall = true;
    public bool justCaughtBall = false;

    public string actionName => "Recieve Ball";
    CharaHitPoints hp => GetComponent<CharaHitPoints>();

    public Action<int> ConsumeEnergy { set; get; }
    public Func<int, bool> CanConsumeEnergy { set; get; }
    public Func<bool> AllowRegen => () => !(IsDetecting && isButtonClicked);

    float newCatchCounter = 0;
    [SerializeField] float reductionCounter = 0;

    void OnEnable() => hp.OnHpSubtracted += DisableDetection;
    void OnDisable() => hp.OnHpSubtracted -= DisableDetection;
    void Update()
    {
        if (GetComponent<BallGrabber>().HasBall)
            return;
        if (!IsDetecting)
        {
            SetIsBallIn(false);
            return;
        }

        if (IsDetecting && isButtonClicked)
        {
            float energyFraction = Time.deltaTime * energyCostPerSec;
            reductionCounter = reductionCounter + Time.deltaTime * energyCostPerSec;
            GameExtentions.StripFloat(reductionCounter, out int ints, out float frac);
            if (CanConsumeEnergy(ints))
            {
                ConsumeEnergy(ints);
                reductionCounter = frac;
            }
            else
                reductionCounter = 0;
        }
        if (!CanConsumeEnergy(1))
            return;

        Bounds b = ballRecieveZone.GetCollider.bounds;
        Collider[] overlaps = Physics.OverlapBox(b.center, b.extents);
        foreach (var col in overlaps)
        {
            Dodgeball ball = col.GetComponent<Dodgeball>();
            if (!ball)
                continue;
            if (!ball.GetComponent<DodgeballThrowSetter>().GetLastSelectedThrowData().canBeCaught)
                continue;

            SetIsBallIn(true);
            return;
        }
        SetIsBallIn(false);
    }
    void FixedUpdate()
    {
        if (justCaughtBall)
        {
            newCatchCounter = newCatchCounter + Time.deltaTime / timeToConsiderAsNewCatch;
            if (newCatchCounter > 1)
            {
                newCatchCounter = 0;
                justCaughtBall = false;
            }
        }
        if (GetComponent<BallGrabber>().HasBall)
            return;
        C_RecieveBall();

    }
    public void Cancel() { }

    public void C_RecieveBall()
    {
        if (CanRecieveBallNow)
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
        justCaughtBall = true;
    }
    public void EnableDetection()
    {
        isDetecting = true;
        reductionCounter = 0;
    }
    public void DisableDetection()
    {
        isDetecting = false;
        reductionCounter = 0;
    }

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