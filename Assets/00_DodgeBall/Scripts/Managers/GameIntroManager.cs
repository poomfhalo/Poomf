using System;
using System.Linq;
using GW_Lib;
using GW_Lib.Utility;
using UnityEngine;

public enum GameType { OneOnOne, TwoOnTwo, ThreeOnThree }
public class GameIntroManager : Singleton<GameIntroManager>
{
    public event Action OnEntryCompleted = null;

    public bool extActivateOnStart = true;
    [Header("Ball Launch Data")]
    [SerializeField] float timeBeforeBallLaunch = 1f;
    [SerializeField] float launchGravity = -20;
    [SerializeField] float ballLaunchHeigth = 6;
    [SerializeField] GameObject ballLauncher = null;

    [Header("Intro Data")]
    [SerializeField] Reactor introReactor;
    [SerializeField] GameStartTextsAnim textAnims = null;

    void Start()
    {
        Dodgeball.instance.gameObject.SetActive(false);
        if (extActivateOnStart)
        {
            StartGame();
            if (!FindObjectOfType<PC>())
                return;

            TeamTag team = TeamsManager.GetTeam(FindObjectOfType<PC>().GetComponent<DodgeballCharacter>()).teamTag;
            var p = FindObjectsOfType<TaggedSpawnPoint>().ToList().Find(s => s.HasTag("MainCamera") && s.BelongsTo(team));
            p.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 15;
        }
    }
    public void StartGame()
    {
        if (introReactor && MatchState.Instance.IsFirstRound)
        {
            introReactor.React();
            introReactor.onCompleted.AddListener(OnReactorCompleted);
        }
        else if (introReactor && !MatchState.Instance.IsFirstRound)
        {
            GameExtentions.SetCharasLock(true, null);
            textAnims.Play(()=> {
                GameExtentions.SetCharasLock(false, null);
                OnReactorCompleted(); 
            });
        }
        else if(introReactor == null)
        {
            OnReactorCompleted();
        }
    }

    private void StartBallLaunch()
    {
        OnEntryCompleted?.Invoke();
        Dodgeball.instance.gameObject.SetActive(true);
        this.InvokeDelayed(timeBeforeBallLaunch, () =>
        {
            Dodgeball.instance.launchUp.C_LaunchUp(ballLaunchHeigth, launchGravity);
            ballLauncher.SetActive(false);
        });
    }

    private void OnReactorCompleted() => this.InvokeDelayed(0.5f, StartBallLaunch);
}