using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using GW_Lib;

/// <summary>
/// A simple script, responsible, for managing, connecting, players, and waiting, untill
/// there are enough players, in case we start in MP_Game scene instead of the MP_Launcher
/// </summary>

public class N_ShortStarter : MonoBehaviourPunCallbacks
{
    [SerializeField] bool autoStart = true;
    [SerializeField] TextMeshProUGUI isMasterText = null;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            if (autoStart)
            {
                PhotonNetwork.NickName = "Player(" + UnityEngine.Random.Range(-100, 100) + ")";
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Log.Warning("Must be connected, for Networked game manager to work");
            }
            return;
        }
        StartGame();
    }
    public override void OnConnectedToMaster()
    {
        Log.Message(PhotonNetwork.NickName + " Connected to Master, Attempting joining random room");
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Log.LogL0(PhotonNetwork.NickName + " failed to join random room, creating Dev Room");
        PhotonNetwork.CreateRoom("Dev Room", N_Lobby.GetDefOptions());
    }
    public override void OnJoinedRoom()
    {
        var room = PhotonNetwork.CurrentRoom;
        Log.LogL0(PhotonNetwork.NickName + " joined room " + room.Name + " now there are " + room.PlayerCount + " in room ");
        if (autoStart)
        {
            Log.Message("Waiting for second player");
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Log.LogL0(newPlayer.NickName + " Have Joined Total Count :: " + PhotonNetwork.CurrentRoom.PlayerCount);
        photonView.RPC("StartGame", RpcTarget.AllViaServer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        this.InvokeDelayed(0.1f, () => {
            TeamTag t = N_TeamsManager.GetTeam(otherPlayer.ActorNumber);
            //DodgeballCharacter chara = N_TeamsManager.GetPlayer(otherPlayer.ActorNumber).GetComponent<DodgeballCharacter>();
            Team team = TeamsManager.GetTeam(t);
            team.CleanUp();
            bool isTeamAEmpty = team.IsEmpty;
            team = TeamsManager.GetNextTeam(team);
            team.CleanUp();
            bool isTeamBEmpty = team.IsEmpty;

            MPTeam mpT = N_TeamsManager.GetMPTeam(otherPlayer.ActorNumber);
            mpT.CleanUp();

            if (isTeamAEmpty || isTeamBEmpty)
            {
                Log.Warning("Game Is over, one of the teams is empty");
                PhotonNetwork.LoadLevel("Menu");
                PhotonNetwork.LeaveRoom();
            }
        });
    }


    [PunRPC]
    private void StartGame()
    {
        GetComponent<N_GameManager>().Initialize();
        if (PhotonNetwork.IsMasterClient)
        {
            isMasterText.text = "Is Master";
        }
        else
        {
            isMasterText.text = "Is Client";
        }
    }
}