using System.Collections.Generic;
using System.Linq;
using GW_Lib;
using GW_Lib.Utility;
using UnityEngine;
using TMPro;
using System;

public class DodgeballGameManager : Singleton<DodgeballGameManager>
{
    [SerializeField] float timeBeforeBallLaunch = 1f;
    [SerializeField] float launchGravity = -20;
    [SerializeField] float ballLaunchHeigth = 6;
    [SerializeField] GameObject ballLauncher = null;
    [SerializeField] bool launchBallOnStart = false;
    [Header("Build Settings")]
    [SerializeField] string build = "Build : 0.0001";
    [SerializeField] TextMeshProUGUI buildText = null;

    BallThrowData[] throws = new BallThrowData[0];

    void Start()
    {
        Dodgeball.instance.gameObject.SetActive(false);
        if (launchBallOnStart)
            StartBallLaunch();

        buildText.text = build;
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

    public void OnBallThrownAtEnemy(DodgeballCharacter by)
    {
        Team team = TeamsManager.GetNextTeam(by);
        team.players.ForEach(p => p.C_BraceForContact());
    }
    public void OnBallCaught(DodgeballCharacter by)
    {
        Team team = TeamsManager.GetTeam(by);
        team.players.ForEach(p => p.C_ReleaseFromBrace());
    }
    public void OnBallHitGround()
    {
        if (Dodgeball.instance.holder == null)
            return;

        Team t = TeamsManager.GetTeam(Dodgeball.instance.holder);
        t.players.ForEach(p => p.C_ReleaseFromBrace());
        t = TeamsManager.GetNextTeam(Dodgeball.instance.holder);
        t.players.ForEach(p => p.C_ReleaseFromBrace());
    }

    public static SpawnPoint GetSpawnPosition(TeamTag team)
    {
        List<SpawnPoint> playerSpawnPoints = FindObjectsOfType<SpawnPoint>().ToList();
        List<SpawnPoint> spawnPoints = playerSpawnPoints.FindAll(p => p.CheckTeam(team));
        SpawnPoint s = null;

        int maxTries = 60;
        do
        {
            int i = UnityEngine.Random.Range(0, spawnPoints.Count);
            s = spawnPoints[i];
            maxTries = maxTries - 1;
            if (maxTries <= 0)
                break;
        } while (s == null || s.HasPlayer);
        return s;
    }
    public static BallThrowData GetThrow(byte id)
    {
        if (instance.throws == null || instance.throws.Length == 0)
        {
            instance.throws = Resources.LoadAll<BallThrowData>("");
        }
        foreach (BallThrowData t in instance.throws)
        {
            if (t.id == id)
                return t;
        }
        return null;
    }
}