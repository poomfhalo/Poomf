using System;
using System.Collections.Generic;

public enum TeamTag { A, B }

[System.Serializable]
public class Team
{
    public List<DodgeballCharacter> players = new List<DodgeballCharacter>();
    public List<string> playersNames = new List<string>();
    public TeamTag teamTag = TeamTag.A;
    public Team(TeamTag teamTag)
    {
        this.teamTag = teamTag;
    }

    public void Join(DodgeballCharacter player)
    {
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
        if(!player)
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
}