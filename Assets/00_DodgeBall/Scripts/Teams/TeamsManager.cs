using System.Collections.Generic;
using System.Linq;
using GW_Lib.Utility;
using UnityEngine;

public class TeamsManager : Singleton<TeamsManager>
{
    public List<DodgeballCharacter> AllCharacters
    {
        get
        {
            allCharacters.RemoveAll(c => c == null);
            return allCharacters;
        }
    }

    [SerializeField] List<DodgeballCharacter> allCharacters = new List<DodgeballCharacter>();
    [SerializeField] List<Team> teams = new List<Team> { new Team(TeamTag.A), new Team(TeamTag.B) };

    public static void JoinTeam(TeamTag team, DodgeballCharacter player)
    {
        Team wantedTeam = instance.teams.Single(t => t.teamTag == team);
        wantedTeam.Join(player);
    }

    public static bool AreFriendlies(DodgeballCharacter chara1, DodgeballCharacter chara2)
    {
        if (chara1 == null || chara2 == null)
            return false;

        Team team1 = GetTeam(chara1);
        Team team2 = GetTeam(chara2);
        bool sameTeam = team1.teamTag == team2.teamTag;

        return sameTeam;
    }
    public static DodgeballCharacter GetNextFriendly(DodgeballCharacter ofThis,DodgeballCharacter curr)
    {
        Team thisTeam = GetTeam(ofThis);
        DodgeballCharacter next = null;
        DodgeballCharacter testableChara = curr;
        do
        {
            next = thisTeam.GetNext(testableChara);
            testableChara = next;
        } while (next == ofThis);
        return next;
    }
    public static DodgeballCharacter GetNextEnemy(DodgeballCharacter ofThis, DodgeballCharacter curr)
    {
        Team t = GetNextTeam(ofThis);
        DodgeballCharacter next = t.GetNext(curr);
        return next;
    }
    public static Team GetNextTeam(DodgeballCharacter chara)
    {
        Team team = GetTeam(chara);
        int i = instance.teams.IndexOf(team);
        i = (i + 1) % instance.teams.Count;
        team = instance.teams[i];
        return team;
    }
    public static Team GetTeam(DodgeballCharacter chara)
    {
        Team team = instance.teams.Single(t => t.palyersNames.Contains(chara.charaName));
        return team;
    }
    public static void AddCharacter(DodgeballCharacter dodgeballCharacter)
    {
        if (instance.allCharacters.Contains(dodgeballCharacter))
            return;

        instance.allCharacters.Add(dodgeballCharacter);
    }
}