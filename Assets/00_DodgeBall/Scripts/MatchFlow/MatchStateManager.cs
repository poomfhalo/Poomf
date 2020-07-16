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
    public Func<bool, IEnumerator> resultLoadFunc = null;
    MatchState matchState => MatchState.Instance;

    [Header("Read Only")]
    public bool extCanPrepareOnStart = true;

    void Start()
    {
        if (extCanPrepareOnStart)
            PrerpareForGame();
    }
    public void PrerpareForGame()
    {
        ConnectToPlayers();
    }
    private void ConnectToPlayers()
    {
        TeamsManager.instance.AllCharacters.ForEach(c =>{
            c.GetComponent<CharaKnockoutPlayer>().E_OnKnockedOut += OnCharaKnockedOut;
        });
    }
    private void OnCharaKnockedOut(DodgeballCharacter charaKnockedOut)
    {
        TeamsManager.GetEmptyTeams(out bool isTeamAEmpty, out bool isTeamBEmpty);
        if (!isTeamAEmpty && !isTeamBEmpty)
            return;

        bool isFinalRound = matchState.IsFinalRound();

        TeamTag winningTeam = TeamTag.A;
        if (isTeamAEmpty)
            winningTeam = TeamTag.B;
        matchState.SetRoundWinner(winningTeam);
        bool isGameOver = isFinalRound || MatchState.Instance.HasTeamWonOverHalf;

        StartCoroutine(ResultLoadFunc(isGameOver));
    }
    IEnumerator RoundEndLoad(bool isFinalRound)
    {
        SceneFader.instance.FadeIn(1, null);
        yield return new WaitForSeconds(1.1f);

        if (isFinalRound)
        {
            SceneManager.LoadScene("SP_MatchResult");
        }
        else
        {
            SceneManager.LoadScene("SP_RoundResult");
        }
    }
}