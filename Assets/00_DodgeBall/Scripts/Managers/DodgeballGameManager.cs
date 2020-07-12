﻿using System.Collections.Generic;
using System.Linq;
using GW_Lib.Utility;
using UnityEngine;
using TMPro;

public class DodgeballGameManager : Singleton<DodgeballGameManager>
{
    public GameSlotsData gameSlotsData => m_gameSlotsData;
    [Header("Game Settings")]
    [SerializeField] GameSlotsData m_gameSlotsData = null;
    [Header("Build Settings")]
    [SerializeField] string build = "Build : 0.0001";
    [SerializeField] TextMeshProUGUI buildText = null;

    [Header("Read Only")]
    [SerializeField] List<Dodgeball> balls = new List<Dodgeball>();

    BallThrowData[] throws = new BallThrowData[0];

    void Start()
    {
        buildText.text = build;
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


    //Helper Functions:
    public static SpawnPath GetSpawnPosition(TeamTag team)
    {
        List<SpawnPath> playerSpawnPoints = FindObjectsOfType<SpawnPath>().ToList();
        List<SpawnPath> spawnPoints = playerSpawnPoints.FindAll(p => p.CheckTeam(team));
        SpawnPath s = null;

        int maxTries = 300;
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
    public static CharaController GetCharaOfSlot(int slotID)
    {
        CharaSlot charaSlot = FindObjectsOfType<CharaSlot>().ToList().Find(s => s.GetID == slotID);
        if(charaSlot)
            return charaSlot.GetComponent<CharaController>();

        return null;
    }
    public static List<SpawnPath> GetPathsOfSlot(int slotID)
    {
        List<SpawnPath> allPaths = FindObjectsOfType<SpawnPath>().ToList();
        return allPaths.FindAll(s => s.CheckSlot(slotID));
    }
}