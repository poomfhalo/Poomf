using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Dodgeball/Match State", fileName = "Match State")]
public class MatchState : PersistantSO
{
    public string GetMatchSceneName => matchSceneName;
    public int PreviousRoundNum => currRound - 1;
    public static MatchState Instance => Resources.Load<MatchState>("MatchState");
    public int TotalRoundsCount => totalRoundsCount;
    public List<int> GetTeamAScore => teamARounds;
    public List<int> GetTeamBScore => teamBRounds;
    public int GetTeamAWins => GetTeamAScore.Count(i => i == 1);
    public int GetTeamBWins => GetTeamBScore.Count(i => i == 1);
    public bool HasTeamWonOverHalf
    {
        get
        {
            int minWinsCount = (TotalRoundsCount / 2) + 1;
            if (GetTeamAWins >= minWinsCount)
                return true;
            if (GetTeamBWins >= minWinsCount)
                return true;

            return false;
        }
    }
    public bool IsFirstRound => currRound == 1;

    [Tooltip("To determine how many rounds is a match consisting of")]
    [SerializeField] int totalRoundsCount = 2;

    [Header("Read Only")]
    [SerializeField] List<int> teamARounds = new List<int>();
    [SerializeField] List<int> teamBRounds = new List<int>();
    [SerializeField] int currRound = 0;
    [SerializeField] string matchSceneName = "";

    public void SetRoundAsTie()
    {
        GetWinsList(TeamTag.A).Add(0);
        GetWinsList(TeamTag.B).Add(0);
        currRound = currRound + 1;
    }
    public void SetRoundWinner(TeamTag toTeam)
    {
        GetWinsList(toTeam).Add(1);
        GetWinsList(TeamsManager.GetNextTeam(toTeam).teamTag).Add(0);
        currRound = currRound + 1;
    }

    public bool IsFinalRound() => currRound >= totalRoundsCount;
    public TeamTag GetMatchWinner()
    {
        TeamTag winner = TeamTag.None;
        if (GetTeamAWins > GetTeamBWins)
            winner = TeamTag.A;
        if (GetTeamAWins < GetTeamBWins)
            winner = TeamTag.B;
        else if (GetTeamAWins == GetTeamBWins)
            winner = TeamTag.None;
        return winner;
    }
    public TeamTag GetRoundWinner(int round)
    {
        round = Mathf.Clamp(round, 1, TotalRoundsCount);
        bool hasALost = teamARounds[round - 1] == 0;
        bool hasBLost = teamBRounds[round - 1] == 0;
        if (hasALost && hasBLost)
            return TeamTag.None;
        if (hasALost)
            return TeamTag.B;
        return TeamTag.A;
    }
    public void StartNewGame(string sceneOfGame)
    {
        ClearScore();
        matchSceneName = sceneOfGame;
    }
    public void SetWinner(TeamTag winnerTeam)
    {
        ClearScore();
        while (currRound<=totalRoundsCount)
        {
            SetRoundWinner(winnerTeam);
            currRound = currRound + 1;
        }
    }

    private List<int> GetWinsList(TeamTag ofTeam)
    {
        switch (ofTeam)
        {
            case TeamTag.A:
                return teamARounds;
            case TeamTag.B:
                return teamBRounds;
        }
        return null;
    }
    private void ClearScore()
    {
        teamARounds.Clear();
        teamBRounds.Clear();
        currRound = 1;
    }
}