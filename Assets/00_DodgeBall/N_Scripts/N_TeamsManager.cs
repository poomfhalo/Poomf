using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class N_TeamsManager : N_Singleton<N_TeamsManager>, IOnEventCallback
{
    [Header("Read Only")]
    [SerializeField] List<MPTeam> mpTeams = new List<MPTeam>();
    [SerializeField] int creationsCount = 0;
    [SerializeField] int playersCount = 0;

    Dictionary<int,int[]> GetMPTeamsData
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
        playersCount = playersCount + 1;
    }
    public void SpreadTeamsData()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        photonView.RPC("RecieveTeamsData", RpcTarget.Others, GetMPTeamsData);
    }

    public static TeamTag GetTeam(int actorID)
    {
        return TeamTag.A;
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
            foreach (var actorNum in t.actors)
            {
                DodgeballCharacter chara = N_Extentions.GetCharacter(actorNum);
                TeamsManager.JoinTeam(t.t, chara);
            }
        }
    }

    //Call backs
    public void OnEvent(EventData photonEvent)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (photonEvent.Code != N_GameManager.OnCreatedPC)
            return;
        creationsCount = creationsCount + 1;
        if (creationsCount < playersCount)
            return;
        photonView.RPC("SyncWithTeamsManager", RpcTarget.All);
    }

    //Helper Functions
    private MPTeam GetMPTeam(TeamTag team)
    {
        return mpTeams.Find(t => t.t == team);
    }
}