using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using System.Linq;
using Photon.Realtime;
using ExitGames.Client.Photon;

public enum N_Prefab { PlayerManager,Player }
/// <summary>
/// Script Flow:
/// 1.Initialize Is called from N_ShortStarter
/// 2.Which Creates PCS
/// 3.Triggers OnEvent N_OnCreatedPC
/// 4.after all are created, we syncronize N_TeamsManager with TeamsManager on all clients
/// 5.Triggers N_OnTeamsAreSynced
/// </summary>
public class N_GameManager : N_Singleton<N_GameManager>, IOnEventCallback,IPunObservable
{
    public float LastLag { private set; get; }

    #region Properties
    public GameObject localPlayer = null;
    public const byte N_OnCreatedPC = 0;
    public const byte N_OnTeamsAreSynced = 1;
    public static event Action OnTeamsAreSynced = null;
    public static event Action OnGameInitialized = null;
    public List<LoadablePrefab> Prefabs => prefabs;
    [SerializeField] List<LoadablePrefab> prefabs = new List<LoadablePrefab> { new LoadablePrefab(N_Prefab.Player, "N_PlayerManager") };
    #endregion

    #region UnityFunctions
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
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }
    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnTeamsAreSynced = null;
        OnGameInitialized = null;
    }
    #endregion

    public void Initialize()
    {
        if(!localPlayer)
            localPlayer = N_Extentions.N_MakeObj(N_Prefab.PlayerManager, Vector3.zero, Quaternion.identity);

        Debug.Log("game initializd event");
        OnGameInitialized?.Invoke();

        if (!PhotonNetwork.IsMasterClient)
            return;

        N_TeamsManager teams = GetComponent<N_TeamsManager>();
        var players = PhotonNetwork.PlayerList;
        foreach (var p in players)
        {
            if (p.ActorNumber % 2 == 0)
            {
                Debug.Log(p.NickName + " Added to team A with num :: " + p.ActorNumber);
                teams.AddPlayer(TeamTag.A, p.ActorNumber);
            }
            else
            {
                Debug.Log(p.NickName + " Added to team B with num :: " + p.ActorNumber);
                teams.AddPlayer(TeamTag.B, p.ActorNumber);
            }
        }

        teams.GetComponent<PhotonView>().RPC("RecieveTeamsData", RpcTarget.Others, teams.GetMPTeamsData);
    }

    //Events Connections
    int createdPCS = 0;
    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case N_OnTeamsAreSynced:
                OnTeamsAreSynced?.Invoke();
                break;
        }
        if (!PhotonNetwork.IsMasterClient)
            return;
        switch (photonEvent.Code)
        {
            case N_OnCreatedPC:
                createdPCS = createdPCS + 1;
                if (createdPCS >= PhotonNetwork.PlayerList.Length)
                {
                    N_TeamsManager.instance.GetComponent<PhotonView>().RPC("SyncWithTeamsManager", RpcTarget.All);
                }
                break;
            case N_OnTeamsAreSynced:
                M_SetUpStartingPositions();
                M_PreparePlayersForGame();
                M_PrepareForGame();
                break;
        }
    }

    //Master Functions
    private void M_SetUpStartingPositions()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            TeamTag t = N_TeamsManager.GetTeam(player.ActorNumber);
            SpawnPoint s = DodgeballGameManager.GetSpawnPosition(t);
            s.GetComponent<PhotonView>().RPC("Fill", RpcTarget.All, player.ActorNumber);
        }
    }
    private void M_PreparePlayersForGame()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            N_PC n_pc = N_TeamsManager.GetPlayer(player.ActorNumber);
            n_pc.GetComponent<PhotonView>().RPC("PrepareForGame", RpcTarget.All);
        }
    }
    private void M_PrepareForGame()
    {
        photonView.RPC("PrepareForGame", RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void PrepareForGame()
    {
        DodgeballGameManager.instance.StartBallLaunch();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        LastLag = (float)(PhotonNetwork.Time - info.SentServerTime);
    }
}