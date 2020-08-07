using System.Collections.Generic;
using GW_Lib.Utility;
using UnityEngine;
using TMPro;
using System;

public class DodgeballGameManager : Singleton<DodgeballGameManager>
{
    public event Action<Dodgeball> onBallAdded = null;
    public event Action<Dodgeball> onBallRemoved = null;

    public List<Dodgeball> GetBalls => balls;
    public GameSlotsData gameSlotsData => m_gameSlotsData;
    public DodgeballCharacter GetLocalPlayer => player;

    [Header("Game Settings")]
    [SerializeField] GameSlotsData m_gameSlotsData = null;
    [Header("Build Settings")]
    [SerializeField] string build = "Build : 0.0001";
    [SerializeField] TextMeshProUGUI buildText = null;

    [Header("Read Only")]
    [SerializeField] List<Dodgeball> balls = new List<Dodgeball>();
    public bool extSetPlayerOnStart = true;
    [SerializeField] DodgeballCharacter player = null;

    void Start()
    {
        buildText.text = build;
        if (extSetPlayerOnStart)
        {
            PrepareForGame(FindObjectOfType<PC>().GetComponent<DodgeballCharacter>());
            TeamsManager.instance.AllCharacters.ForEach(c => {
                c.PrepareForGame();
            });
        }
    }

    public void OnBallThrownAtAlly(DodgeballCharacter by)
    {
        Team team = TeamsManager.GetTeam(by);
        team.players.ForEach(p => {
            if (p != by)
                p.C_EnableBallReciption();
        });
    }
    public void OnBallThrownAtEnemy(DodgeballCharacter by)
    {
        Team team = TeamsManager.GetNextTeam(by);
        team.players.ForEach(p =>{
            //p.C_BraceForContact();
            p.C_EnableHitDetection();
            p.C_EnableBallReciption();
        });
    }
    public void OnBallCaught(DodgeballCharacter by)
    {
        Team team = TeamsManager.GetTeam(by);
        team.players.ForEach(p => {
            p.C_DisableHitDetection();
            p.C_DisableBallReciption();
            //p.C_ReleaseFromBrace();
        });
    }
    public void OnBallHitGround()
    {
        if (Dodgeball.instance.holder == null)
            return;

        Log.LogL0("GameManager().OnBallHitGround");
        Team t = TeamsManager.GetTeam(Dodgeball.instance.holder);
        t.players.ForEach(p => { 
            //p.C_ReleaseFromBrace();
            p.C_DisableHitDetection();
            p.C_DisableBallReciption(); 
        });
        t = TeamsManager.GetNextTeam(Dodgeball.instance.holder);
        t.players.ForEach(p => { 
            //p.C_ReleaseFromBrace();
            p.C_DisableHitDetection();
            p.C_DisableBallReciption();
        });
    }


    //Helper Functions:
    public static void PrepareForGame(DodgeballCharacter player)
    {
        instance.player = player;
    }
    public static void AddBall(Dodgeball b)
    {
        if (!instance.balls.Contains(b))
        {
            instance.balls.Add(b);
            instance.onBallAdded?.Invoke(b);
        }
    }
    public static void RemoveBall(Dodgeball b)
    {
        if (instance && instance.balls.Contains(b))
        {
            instance.balls.Remove(b);
            instance.onBallRemoved?.Invoke(b);
        }
    }
    public static Dodgeball GetClosestBall(Transform closestTo)
    {
        List<Dodgeball> balls = instance.balls;
        float minDist = float.MaxValue;
        Dodgeball closestBall = null;
        foreach (var b in balls)
        {
            float d = Vector3.Distance(closestTo.position, b.position);
            if (d <= minDist)
            {
                minDist = d;
                closestBall = b;
            }
        }
        return closestBall;
    }
}