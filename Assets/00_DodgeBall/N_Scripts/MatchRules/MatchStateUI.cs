using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchStateUI : MonoBehaviour
{
    [SerializeField] MatchState matchState = null;
    [SerializeField] RoundsGroupUI teamA = null;
    [SerializeField] RoundsGroupUI teamB = null;

    void Awake()
    {
        teamA.Initialize(matchState.TotalRoundsCount,matchState.GetTeamAScore);
        teamB.Initialize(matchState.TotalRoundsCount,matchState.GetTeamBScore);
    }
}