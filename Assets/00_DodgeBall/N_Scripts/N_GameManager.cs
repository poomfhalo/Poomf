using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using System.Linq;
using Photon.Realtime;

public enum N_Prefab { PlayerManager,Player }

public class N_GameManager : N_Singleton<N_GameManager>
{
    public List<LoadablePrefab> Prefabs => prefabs;
    [SerializeField] bool autoStart = true;
    [SerializeField] List<LoadablePrefab> prefabs = new List<LoadablePrefab> { new LoadablePrefab(N_Prefab.Player,"N_PlayerManager") };

    void Reset()
    {
        prefabs = new List<LoadablePrefab>();
        foreach (var e in Enum.GetValues(typeof(N_Prefab)).Cast<N_Prefab>())
        {
            prefabs.Add(new LoadablePrefab(e,"N_" + e.ToString()));
        }
    }
    void Start()
    {
        N_Extentions.prefabs = prefabs;
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
        autoStart = false;
        GameObject g = N_Extentions.N_MakeObj(N_Prefab.PlayerManager, Vector3.zero, Quaternion.identity);
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
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " Have Joined Total Count :: " + PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("CreatePlayerManager", RpcTarget.All);
        }
    }
    public override void OnJoinedRoom()
    {
        var room = PhotonNetwork.CurrentRoom;
        Debug.Log(PhotonNetwork.NickName + " joined room " + room.Name + " now there are " + room.PlayerCount + " in room ");
        if(autoStart)
        {
            Debug.Log("Waiting for second player");
        }
    }

    [PunRPC]
    private void CreatePlayerManager()
    {
        GameObject g = N_Extentions.N_MakeObj(N_Prefab.PlayerManager, Vector3.zero, Quaternion.identity);
        Debug.Log("Created PlayerManager " + PhotonNetwork.LocalPlayer.NickName, g);
    }
}