using System;
using System.Collections.Generic;
using System.Linq;
using GW_Lib;
using GW_Lib.Utility;
using UnityEngine;

public enum GameType { OneOnOne, TwoOnTwo, ThreeOnThree }
public class GameIntroManager : Singleton<GameIntroManager>
{
    [Header("Ball Launch Data")]
    [SerializeField] float timeBeforeBallLaunch = 1f;
    [SerializeField] float launchGravity = -20;
    [SerializeField] float ballLaunchHeigth = 6;
    [SerializeField] GameObject ballLauncher = null;

    [Header("Intro Data")]
    [SerializeField] Reactor introReactor;
    void Start()
    {
        Dodgeball.instance.gameObject.SetActive(false);
        if (introReactor)
        {
            introReactor.React();
            introReactor.onCompleted.AddListener(OnCompleted);
        }
    }

    public void StartBallLaunch()
    {
        Dodgeball.instance.gameObject.SetActive(true);
        this.InvokeDelayed(timeBeforeBallLaunch, () =>
        {
            Dodgeball.instance.launchUp.C_LaunchUp(ballLaunchHeigth, launchGravity);
            ballLauncher.SetActive(false);
        });
    }
    private void OnCompleted()
    {
        StartBallLaunch();
    }
}