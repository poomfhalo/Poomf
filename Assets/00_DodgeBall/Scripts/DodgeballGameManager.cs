using System.Collections.Generic;
using System.Linq;
using GW_Lib;
using GW_Lib.Utility;
using UnityEngine;

public class DodgeballGameManager : Singleton<DodgeballGameManager>
{
    [SerializeField] float timeBeforeBallLaunch = 1f;
    [SerializeField] float launchGravity = -20;
    [SerializeField] float ballLaunchHeigth = 6;
    [SerializeField] GameObject ballLauncher = null;
    [SerializeField] bool launchBallOnStart = false;

    void Start()
    {
        Dodgeball.instance.gameObject.SetActive(false);
        if (launchBallOnStart)
            StartBallLaunch();
    }

    public void StartBallLaunch()
    {
        Dodgeball.instance.gameObject.SetActive(true);
        this.InvokeDelayed(timeBeforeBallLaunch, () => {
            Dodgeball.instance.LaunchUp(ballLaunchHeigth, launchGravity);
            ballLauncher.SetActive(false);
        });
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
}