using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using System.Linq;
using Photon.Realtime;

/// <summary>
/// How does mp work?
/// 1.ShortStarter handles, starting when there are enough players, whether we came from MP_Launcher or MP_Game
/// 2.CreatePlayers is called, which handles, creating all N_PlayersManagers along side.
/// 3.We then call SetUpTeams only on the Master Client.
/// 4.once Teams for PlayerManagers are assigned, we spread data to other clients
/// 5.each player Manager, handles creating his own PC/N_PC, no need to transfer ownership, as its handled by photon.
/// 6.Once PC is created, its disabled, and on server, we recieve an event called OnCreatedPC
/// 7.once all players have a PC, we Sync the N_TeamsManager multiplayer connected teams, to local TeamsManager
/// 8.we assign the starting positions
/// </summary>

public enum N_Prefab { PlayerManager,Player }

public class N_GameManager : N_Singleton<N_GameManager>
{
    public GameObject localPlayer = null;
    public static RaiseEventOptions GetDefOps
    {
        get
        {
            RaiseEventOptions ops = new RaiseEventOptions();
            ops.Receivers = ReceiverGroup.All;
            return ops;
        }
    }
    public static byte OnCreatedPC = 0;
    public List<LoadablePrefab> Prefabs => prefabs;
    [SerializeField] List<LoadablePrefab> prefabs = new List<LoadablePrefab> { new LoadablePrefab(N_Prefab.Player, "N_PlayerManager") };

    void Reset()
    {
        prefabs = new List<LoadablePrefab>();
        foreach (var e in Enum.GetValues(typeof(N_Prefab)).Cast<N_Prefab>())
        {
            prefabs.Add(new LoadablePrefab(e, "N_" + e.ToString()));
        }
    }
    protected override void Awake()
    {
        base.Awake();
        N_Extentions.prefabs = prefabs;
    }

    public void SetUpTeams()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        N_TeamsManager teams = GetComponent<N_TeamsManager>();
        var players = PhotonNetwork.PlayerList;
        foreach (var p in players)
        {
            if (p.ActorNumber % 2 == 0)
            {
                Debug.Log(p.NickName + " Joined team A");
                teams.AddPlayer(TeamTag.A, p.ActorNumber);
            }
            else
            {
                Debug.Log(p.NickName + " Joined team B");
                teams.AddPlayer(TeamTag.B, p.ActorNumber);
            }
        }
        teams.SpreadTeamsData();
    }
    public void CreatePlayerManager()
    {
        localPlayer = N_Extentions.N_MakeObj(N_Prefab.PlayerManager, Vector3.zero, Quaternion.identity);
    }
}