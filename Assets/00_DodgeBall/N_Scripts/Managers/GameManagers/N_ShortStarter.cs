﻿using UnityEngine;
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
    [SerializeField] TextMeshProUGUI pingText = null;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            if (autoStart)
            {
                PhotonNetwork.NickName = "Player(" + UnityEngine.Random.Range(-100, 100) + ")";
                PhotonNetwork.ConnectUsingSettings();
                FindObjectOfType<MatchStateManager>().StartNewGame();
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
        Log.Message(PhotonNetwork.NickName + " Connected to Master, in " + PhotonNetwork.CloudRegion + " :: Attempting joining random room ");
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

    void Update()
    {
        pingText.text = PhotonNetwork.GetPing().ToString();
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