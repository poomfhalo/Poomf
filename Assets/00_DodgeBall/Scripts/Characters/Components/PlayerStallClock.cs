using System;
using UnityEngine;

public class PlayerStallClock : MonoBehaviour
{
    [SerializeField] float stallTime = 20;
    [Header("Read Only")]
    [SerializeField] float stallCounter = 0;

    BallGrabber grabber = null;
    DodgeballCharacter chara = null;
    bool wasCharged = false;

    public Func<bool> ExtAllowForcedThrow = () => true;

    void Start()
    {
        grabber = GetComponent<BallGrabber>();
        chara = GetComponent<DodgeballCharacter>();
        grabber.onBallInHands += OnBallInHands;
    }
    void Update()
    {
        if (!grabber.HasBall)
            return;
        if (wasCharged)
            return;

        stallCounter += Time.deltaTime / stallTime;

        if (stallCounter<1)
            return;
        if (!ExtAllowForcedThrow())
            return;

        stallCounter = 0;
        chara.C_OnBallAction(UnityEngine.InputSystem.InputActionPhase.Started);
        wasCharged = true;
    }

    private void OnBallInHands()
    {
        stallCounter = 0;
        wasCharged = false;
    }
}