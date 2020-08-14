using System;
using System.Collections.Generic;

public enum TeamTag { A, B, None }

[Serializable]
public class Team
{
    public List<DodgeballCharacter> players = new List<DodgeballCharacter>();
    public List<string> playersNames = new List<string>();
    public TeamTag teamTag = TeamTag.A;
    public bool IsEmpty
    {
        get
        {
            CleanUp();
            if (players.Count == 0)
                return true;

            foreach (var p in players)
            {
                if (p.IsInField)
                    return false;
            }
            return true;
        }
    }
    public int GetAliveCount()
    {
        int aliveCount = 0;
        players.ForEach(p => { 
            if(p.IsInField)
                aliveCount = aliveCount + 1;
        });
        return aliveCount;
    }
    public int GetTotalTeamHP()
    {
        int totalTeamHP = 0;
        players.ForEach(p => {
            if (p.IsInField)
                totalTeamHP = totalTeamHP + p.GetComponent<CharaHitPoints>().CurrHP;
        });
        return totalTeamHP;
    }
    public Team(TeamTag teamTag)
    {
        this.teamTag = teamTag;
    }

    public void Join(DodgeballCharacter player)
    {
        CleanUp();

        if (!player)
        {
            UnityEngine.Debug.Log("Null player trying to join " + teamTag);
            return;
        }
        if (IsInTeam(player.charaName))
            return;

        playersNames.Add(player.charaName);
        players.Add(player);
    }
    public bool IsInTeam(string player)
    {
        return playersNames.Contains(player);
    }
    public bool IsInTeam(DodgeballCharacter chara)
    {
        return players.Contains(chara);
    }

    public void Leave(DodgeballCharacter player)
    {
        CleanUp();
        if (!player)
        {
            UnityEngine.Debug.Log("Null player trying to leave " + teamTag);
        }
        if (!IsInTeam(player))
            return;

        playersNames.Remove(player.charaName);
        players.Remove(player);
    }

    public DodgeballCharacter GetNext(DodgeballCharacter curr)
    {
        CleanUp();
        if (players.Count == 0)
            return null;
        if (curr == null)
            return players[0];

        int i = players.IndexOf(curr);
        if (i == -1)
            return null;

        i = (i + 1) % players.Count;
        return players[i];
    }

    public void CleanUp()
    {
        players.RemoveAll(p => p == null);
        playersNames.RemoveAll(NamesCleanUp);
    }

    private bool NamesCleanUp(string n)
    {
        foreach (var p in players)
        {
            if (p.charaName == n)
                return false;
        }
        return true;
    }
}