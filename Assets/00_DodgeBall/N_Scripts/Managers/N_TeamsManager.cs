using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class N_TeamsManager : N_Singleton<N_TeamsManager>
{
    [Header("Read Only")]
    [SerializeField] List<MPTeam> mpTeams = new List<MPTeam>();

    public Dictionary<int,int[]> GetMPTeamsData
    {
        get
        {
            Dictionary<int, int[]> data = new Dictionary<int, int[]>();
            foreach (var t in mpTeams)
            {
                t.FillTeamData(ref data);
            }
            return data;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.AddCallbackTarget(this);
    }
    public override void OnDisable()
    {
        base.OnDisable();
        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void AddPlayer(TeamTag team, int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (GetMPTeam(team) == null)
            mpTeams.Add(new MPTeam(team, new List<int>()));

        GetMPTeam(team).actors.Add(actorNumber);
    }

    [PunRPC]
    private void RecieveTeamsData(Dictionary<int, int[]> teamsData)
    {
        foreach (var v in teamsData)
        {
            MPTeam team = new MPTeam((TeamTag)v.Key,v.Value.ToList());
            mpTeams.Add(team);
        }
    }
    [PunRPC]
    private void SyncWithTeamsManager()
    {
        foreach (var t in mpTeams)
        {
            t.actors.RemoveAll(a => N_Extentions.GetCharacter(a) == null);
        }
        foreach (var t in mpTeams)
        {
            foreach (var actorNum in t.actors)
            {
                DodgeballCharacter chara = N_Extentions.GetCharacter(actorNum);
                TeamsManager.JoinTeam(t.t, chara);
            }
        }
        N_Extentions.N_RaiseEvent(N_GameManager.N_OnTeamsAreSynced,null);
    }

    //Helper Functions
    public static MPTeam GetMPTeam(TeamTag team)
    {
        return instance.mpTeams.Find(t => t.t == team);
    }
    public static MPTeam GetMPTeam(int actorNumber)
    {
        return instance.mpTeams.Find(t => t.actors.Contains(actorNumber));
    }
    public static TeamTag GetTeam(int actorID)
    {
        MPTeam team = GetMPTeam(actorID);
        if (team == null)
            return TeamTag.A;
        return team.t;
    }
    public static N_PC GetPlayer(int actorNumber)
    {
        DodgeballCharacter chara = N_Extentions.GetCharacter(actorNumber);
        if (chara)
        {
            return chara.GetComponent<N_PC>();
        }
        return null;
    }
}