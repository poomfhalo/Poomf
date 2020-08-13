using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SP_PlayerData
{
    public string charaName = "";
    public CharaSkinData skinData;
}
[CreateAssetMenu(fileName = "SP_MatchData", menuName = "Dodgeball/SP_MatchData")]
public class SP_MatchData : ScriptableObject
{
    public MatchType matchType = MatchType.TwoVsTwo;
    public int playerActorID = 0;

    public List<SP_PlayerData> teamA = new List<SP_PlayerData>();
    public List<SP_PlayerData> teamB = new List<SP_PlayerData>();
}