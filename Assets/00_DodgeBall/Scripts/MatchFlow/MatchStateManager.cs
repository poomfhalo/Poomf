using UnityEngine;
using GW_Lib.Utility;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class MatchStateManager : Singleton<MatchStateManager>
{
    public Func<bool,IEnumerator> ResultLoadFunc
    {
        get
        {
            if (resultLoadFunc == null)
                resultLoadFunc = RoundEndLoad;
            return resultLoadFunc;
        }
        set
        {
            resultLoadFunc = value;
        }
    }
    [SerializeField] float gameEndSlowTimeDur = 3;
    public Func<bool, IEnumerator> resultLoadFunc = null;
    MatchState matchState => MatchState.Instance;
    MatchTimeWarper timeWarper => GetComponent<MatchTimeWarper>();

    [Header("Read Only")]
    public bool extCanPrepareOnStart = true;

    void Start()
    {
        if (extCanPrepareOnStart)
            PrerpareForGame();
    }
    public void PrerpareForGame()
    {
        TeamsManager.instance.AllCharacters.ForEach(c => {
            c.GetComponent<CharaKnockoutPlayer>().E_OnKnockedOut += OnCharaKnockedOut;
        });

        FindObjectOfType<RoundTimer>().E_OnTimerCompleted += OnTimerCompleted;
    }

    private void OnTimerCompleted()
    {
        Team a = TeamsManager.GetTeam(TeamTag.A);
        Team b = TeamsManager.GetTeam(TeamTag.B);

        int aAliveCount = a.GetAliveCount();
        int bAliveCount = b.GetAliveCount();
        Team winnerTeam = null;

        if (aAliveCount > bAliveCount)
            winnerTeam = a;
        else if (aAliveCount < bAliveCount)
            winnerTeam = b;
        else
        {
            int teamAHP = a.GetTotalTeamHP();
            int teamBHP = b.GetTotalTeamHP();
            if (teamAHP == teamBHP)
                winnerTeam = null;
            else if (teamAHP > teamBHP)
                winnerTeam = a;
            else if (teamAHP < teamBHP)
                winnerTeam = b;
        }

        timeWarper.SlowTime(gameEndSlowTimeDur, () => EndRound(winnerTeam));
    }

    private void OnCharaKnockedOut(DodgeballCharacter charaKnockedOut)
    {
        TeamsManager.GetEmptyTeams(out bool isTeamAEmpty, out bool isTeamBEmpty);
        if (!isTeamAEmpty && !isTeamBEmpty)
            return;

        TeamTag winningTeam = TeamTag.A;
        if (isTeamAEmpty)
            winningTeam = TeamTag.B;
        Team winnerTeam = TeamsManager.GetTeam(winningTeam);
        timeWarper.SlowTime(gameEndSlowTimeDur, () => EndRound(winnerTeam));
    }
    void EndRound(Team winner)
    {
        bool isFinalRound = matchState.IsFinalRound();

        if(winner == null)
            matchState.SetRoundAsTie();
        else
            matchState.SetRoundWinner(winner.teamTag);
        bool isGameOver = isFinalRound || MatchState.Instance.HasTeamWonOverHalf;

        StartCoroutine(ResultLoadFunc(isGameOver));
    }
    IEnumerator RoundEndLoad(bool isFinalRound)
    {
        SceneFader.instance.FadeIn(1, null);
        yield return new WaitForSeconds(1.1f);

        if (isFinalRound)
            SceneManager.LoadScene("SP_MatchResult");
        else
            SceneManager.LoadScene("SP_RoundResult");
    }
}