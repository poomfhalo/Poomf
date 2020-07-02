using System.Collections.Generic;
using System.Linq;
using GW_Lib;
using GW_Lib.Utility;
using UnityEngine;
using TMPro;

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

    [Header("Read Only")]
    [SerializeField] List<Dodgeball> balls = new List<Dodgeball>();

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

        Log.LogL0("GameManager().OnBallHitGround");
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

        int maxTries = 120;
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

    public static Dodgeball GetClosestBall(Transform closestTo)
    {
        List<Dodgeball> balls = instance.balls;
        float minDist = float.MaxValue;
        Dodgeball closestBall = null;
        foreach (var b in balls)
        {
            float d = Vector3.Distance(closestTo.position, closestBall.position);
            if(d<=minDist)
            {
                minDist = d;
                closestBall = b;
            }
        }
        return closestBall;
    }
    public static void AddBall(Dodgeball b)
    {
        if (!instance.balls.Contains(b))
            instance.balls.Add(b);
    }
    public static void RemoveBall(Dodgeball b)
    {
        if (instance && instance.balls.Contains(b))
            instance.balls.Remove(b);
    }
}