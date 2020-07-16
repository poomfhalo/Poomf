using UnityEngine;

public class MatchStateUI : MonoBehaviour
{
    [SerializeField] RoundsGroupUI teamA = null;
    [SerializeField] RoundsGroupUI teamB = null;

    void Awake()
    {
        teamA.Initialize(MatchState.Instance.TotalRoundsCount, MatchState.Instance.GetTeamAScore);
        teamB.Initialize(MatchState.Instance.TotalRoundsCount, MatchState.Instance.GetTeamBScore);
    }
}