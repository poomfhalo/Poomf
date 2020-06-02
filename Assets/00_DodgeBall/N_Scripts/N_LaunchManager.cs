using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System;
using Photon.Realtime;

public class N_LaunchManager : MonoBehaviourPunCallbacks
{
    [SerializeField] int levelIndex = 1;
    [Header("Start")]
    [SerializeField] GameObject enterGamePanel = null;
    [SerializeField] TMP_InputField playerName = null;
    [SerializeField] Button startGameButton = null;
    [Header("Status Display")]
    [SerializeField] GameObject intermediatePanel = null;
    [Header("Lobby")]
    [SerializeField] GameObject lobbyPanel = null;
    [SerializeField] Button joiningButton = null;

    void Start()
    {
        GenerateRandomName();
        startGameButton.onClick.AddListener(EnterGame);
        joiningButton.onClick.AddListener(JoinOrCreateGame);
        enterGamePanel.SetActive(true);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnected()
    {
        base.OnConnected();
        print("Connected to internet");
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        print("I " + PhotonNetwork.NickName + " have Connected To Master");
        lobbyPanel.SetActive(true);
        intermediatePanel.SetActive(false);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("failed to join room :: " + message);
        string rnd = "Room : " + UnityEngine.Random.Range(-100, 100);
        PhotonNetwork.CreateRoom(rnd, GetDefOptions());
    }
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " have joined room " + PhotonNetwork.CurrentRoom.Name);
        lobbyPanel.SetActive(false);
        Debug.Log("Wait For Second Player");
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " Have Joined Total Count :: " + PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel(levelIndex);
    }


    private void EnterGame()
    {
        if(string.IsNullOrEmpty(playerName.text))
        {
            GenerateRandomName();
        }
        PhotonNetwork.NickName = playerName.text;
        if(!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        enterGamePanel.SetActive(false);
        intermediatePanel.SetActive(true);
    }
    private void GenerateRandomName()
    {
        playerName.text = "XXXX : " + UnityEngine.Random.Range(-500, 500);
    }
    private void JoinOrCreateGame()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public static RoomOptions GetDefOptions()
    {
        RoomOptions ops = new RoomOptions();
        ops.MaxPlayers = 6;
        ops.IsVisible = true;
        ops.IsOpen = true;
        return ops;
    }
}