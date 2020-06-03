using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// A simple script, responsible, for managing, connecting, players, and waiting, untill
/// there are enough players, in case we start in MP_Game scene instead of the MP_Launcher
/// </summary>

public class N_ShortStarter : MonoBehaviourPunCallbacks
{
    [SerializeField] bool autoStart = true;

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
                Debug.LogWarning("Must be connected, for Networked game manager to work");
            }
            return;
        }
        StartGame();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " Connected to Master, Attempting joining random room");
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(PhotonNetwork.NickName + " failed to join random room, creating Dev Room");
        PhotonNetwork.CreateRoom("Dev Room", N_LaunchManager.GetDefOptions());
    }
    public override void OnJoinedRoom()
    {
        var room = PhotonNetwork.CurrentRoom;
        Debug.Log(PhotonNetwork.NickName + " joined room " + room.Name + " now there are " + room.PlayerCount + " in room ");
        if (autoStart)
        {
            Debug.Log("Waiting for second player");
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " Have Joined Total Count :: " + PhotonNetwork.CurrentRoom.PlayerCount);
        photonView.RPC("StartGame", RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GetComponent<N_GameManager>().SetUpTeams();
        }
        GetComponent<N_GameManager>().CreatePlayerManager();
    }
}