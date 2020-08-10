using System;
using System.Collections.Generic;
using GW_Lib;
using UnityEngine;

public class DodgeballThrowSetter : MonoBehaviour
{
    public enum ThrowType { Def, Speeder }
    public event Action E_OnThrowSelected = null;

    [Serializable]
    public class ThrowDataToType
    {
        public BallThrowData throwData = null;
        public ThrowType throwType = ThrowType.Def;
    }

    [SerializeField] float timeToQuickThrow = 1;
    [SerializeField] List<ThrowDataToType> ballThrows = new List<ThrowDataToType>();
    [SerializeField] BallThrowData lastSelectedThrowData = null;

    [Header("Read Only")]
    [SerializeField] bool wasJustCaught = true;

    Dodgeball ball = null;
    DodgeballGoTo goTo = null;
    Coroutine caughtCoro = null;

    void OnEnable()
    {
        ball = GetComponent<Dodgeball>();
        goTo = GetComponent<DodgeballGoTo>();

        goTo.onGoto += OnGoTo;
        ball.E_OnStateUpdated += OnStateUpdated;
    }
    void OnDisable()
    {
        wasJustCaught = false;
        goTo.onGoto -= OnGoTo;
        ball.E_OnStateUpdated -= OnStateUpdated;
    }

    public BallThrowData GetLastSelectedThrowData() => lastSelectedThrowData;
    public void SelectThrowData()
    {
        ThrowType throwType = ThrowType.Def;
        if (wasJustCaught)
            throwType = ThrowType.Speeder;
        ThrowDataToType dataToType = ballThrows.Find(bt => bt.throwType == throwType);
        lastSelectedThrowData = dataToType.throwData;
    }
    public void SetThrowDataTo(byte id)//Used for Multiplayer, in case, there is a miss calculation on the client's end.
    {
        BallThrowData newThrowdata = GetThrowData(id);
        if (GetLastSelectedThrowData() != newThrowdata)
            lastSelectedThrowData = GetThrowData(id);
    }

    private BallThrowData GetThrowData(byte id)
    {
        foreach (ThrowDataToType t in ballThrows)
        {
            if (t.throwData.id == id)
                return t.throwData;
        }
        return null;
    }
    private void OnGoTo()
    {
        if (ball.ballState == Dodgeball.BallState.Flying)
        {
            wasJustCaught = true;
            caughtCoro = this.InvokeDelayed(timeToQuickThrow, () => wasJustCaught = false);
        }
    }
    private void OnStateUpdated(Dodgeball.BallState newState)
    {
        if (newState == Dodgeball.BallState.OnGround || newState == Dodgeball.BallState.Flying)
        {
            wasJustCaught = false;
            this.KillCoro(ref caughtCoro);
        }
    }
}