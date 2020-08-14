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

        Team currTeam = instance.teams.Find(testTeam => testTeam.IsInTeam(player));
        if(currTeam != null)
        {
            currTeam.Leave(player);
        }

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
        thisTeam.CleanUp();

        DodgeballCharacter next = curr;
        int maxTries = 30;
        do
        {
            next = thisTeam.GetNext(curr);
            if (next == ofThis)
                next = thisTeam.GetNext(next);
            maxTries--;
            if (maxTries < 0)
            {
                next = null;
                break;
            }
        } while (next == null || next == ofThis);
        return next;
    }
    public static DodgeballCharacter GetNextEnemy(DodgeballCharacter ofThis, DodgeballCharacter curr,bool onlyInField)
    {
        Team t = GetNextTeam(ofThis);
        DodgeballCharacter next = t.GetNext(curr);
        t.CleanUp();
        int maxTries = 10;
        if (onlyInField)
        {
            while (!next.IsInField)
            {
                next = t.GetNext(next);

                maxTries--;
                if (maxTries < 0)
                {
                    next = null;
                    break;
                }
            }
        }
        return next;
    }
    public static Team GetNextTeam(DodgeballCharacter chara)
    {
        Team team = GetTeam(chara);
        team = GetNextTeam(team);
        return team;
    }
    public static Team GetNextTeam(Team t)
    {
        int i = instance.teams.IndexOf(t);
        i = (i + 1) % instance.teams.Count;
        Team team = instance.teams[i];
        return team;
    }
    public static Team GetNextTeam(TeamTag teamTag)
    {
        return GetNextTeam(GetTeam(teamTag));
    }
    public static Team GetTeam(TeamTag teamTag)
    {
        Team team = instance.teams.Single(t => t.teamTag == teamTag);
        return team;
    }
    public static Team GetTeam(DodgeballCharacter chara)
    {
        Team team = instance.teams.Single(t => t.players.Contains(chara));
        return team;
    }
    public static void AddCharacter(DodgeballCharacter dodgeballCharacter)
    {
        if (instance.allCharacters.Contains(dodgeballCharacter))
            return;

        instance.allCharacters.Add(dodgeballCharacter);
        instance.allCharacters.RemoveAll(c => c == null);
    }

    public static void GetEmptyTeams(out bool isTeamAEmpty, out bool isTeamBEmpty)
    {
        isTeamAEmpty = isTeamBEmpty = false;
        if (!instance)
            return;

        Team testTeam = GetTeam(TeamTag.A);
        isTeamAEmpty = testTeam.IsEmpty;
        testTeam = GetNextTeam(testTeam);
        isTeamBEmpty = testTeam.IsEmpty;
    }
}