using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "DodgeBall/Match State", fileName = "Match State")]
public class MatchState : ScriptableObject
{
    public int TotalRoundsCount => totalRoundsCount;

    public List<int> GetTeamAScore => teamARounds;
    public List<int> GetTeamBScore => teamBRounds;

    public int GetTeamAWins => GetTeamAScore.Count(i => i == 1);
    public int GetTeamBWins => GetTeamBScore.Count(i => i == 1);

    [Tooltip("To determine how many rounds is a match consisting of")]
    [SerializeField] int totalRoundsCount = 2;

    [Header("Read Only")]
    [SerializeField] List<int> teamARounds = new List<int>();
    [SerializeField] List<int> teamBRounds = new List<int>();

    [SerializeField] int currRound = 0;

    public void MoveToNextRound()
    {
        currRound = currRound + 1;
    }
    public void AddWin(TeamTag toTeam)
    {
        GetWinsList(toTeam).Add(1);
        GetWinsList(TeamsManager.GetNextTeam(toTeam).teamTag).Add(0);
        MoveToNextRound();
    }

    public bool IsFinalRound() => currRound >= totalRoundsCount;
    public TeamTag GetWinner(int ofRound)
    {
        if (teamARounds.Contains(ofRound))
            return TeamTag.A;

        return TeamTag.B;
    }
    public void StartNewGame()
    {
        teamARounds.Clear();
        teamBRounds.Clear();
        currRound = 1;
    }
    public void SetWinner(TeamTag winnerTeam)
    {
        while (currRound<=totalRoundsCount)
        {
            AddWin(winnerTeam);
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
}