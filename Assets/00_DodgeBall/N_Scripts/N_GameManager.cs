using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using System.Linq;
using Photon.Realtime;

public enum N_Prefab { PlayerManager,Player }

public class N_GameManager : N_Singleton<N_GameManager>
{
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
        GameObject g = N_MakeObj(N_Prefab.PlayerManager, Vector3.zero, Quaternion.identity);
    }

    public static GameObject N_MakeObj(N_Prefab prefab, Vector3 pos, Quaternion rot, byte group = 0, object[] data = null)
    {
        LoadablePrefab p = GetPrefab(prefab);
        GameObject o = PhotonNetwork.Instantiate(p.name, pos, rot, group, data);
        return o;
    }
    public static GameObject MakeObj(N_Prefab prefab, Vector3 pos, Quaternion rot)
    {
        GameObject g = Instantiate(GetPrefab(prefab).LoadPrefab());
        g.transform.position = pos;
        g.transform.rotation = rot;
        return g;
    }
    public static LoadablePrefab GetPrefab(N_Prefab prefab)
    {
        return instance.prefabs.Single(f => f.type == prefab);
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
    }
    public override void OnJoinedRoom()
    {
        var room = PhotonNetwork.CurrentRoom;

        GameObject g = N_MakeObj(N_Prefab.PlayerManager, Vector3.zero, Quaternion.identity);
        Debug.Log(PhotonNetwork.NickName + " joined room " + room.Name + " now there are " + room.PlayerCount + " in room ", g);
    }
}