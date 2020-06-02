using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using System.Linq;
using Photon.Realtime;

public enum N_Prefab { PlayerManager,Player }

[Serializable]
public class LoadablePrefab
{
    public N_Prefab type = N_Prefab.PlayerManager;
    public string name = "N_PlayerManager";
    public LoadablePrefab(){ }
    public LoadablePrefab(N_Prefab type, string name)
    {
        this.type = type;
        this.name = name;
    }
}

public class N_GameManager : N_Singleton<N_GameManager>
{
    [SerializeField] List<LoadablePrefab> prefabs = new List<LoadablePrefab> { new LoadablePrefab(N_Prefab.Player,"N_PlayerManager") };

    List<SpawnPoint> playerSpawnPoints = new List<SpawnPoint>();
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
        if(!PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("Must be connected, for Networked game manager to work");
            return;
        }

        print(PhotonNetwork.NickName+ " N_GameManager() :: in room " + PhotonNetwork.CurrentRoom.Name);
        playerSpawnPoints = FindObjectsOfType<SpawnPoint>().ToList();
        int i = UnityEngine.Random.Range(0, playerSpawnPoints.Count);
        SpawnPoint s = playerSpawnPoints[i];
        MakeObj(N_Prefab.PlayerManager, s.position, s.rotation);
    }

    public static GameObject MakeObj(N_Prefab prefab, Vector3 pos, Quaternion rot, byte group = 0, object[] data = null)
    {
        LoadablePrefab p = instance.prefabs.Single(f => f.type == prefab);
        GameObject o = PhotonNetwork.Instantiate(p.name, pos, rot, group, data);
        return o;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log(newPlayer.NickName + " Have Joined Total Count :: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        var room = PhotonNetwork.CurrentRoom;
        Debug.Log(PhotonNetwork.NickName + " joined room " + room.Name + " now there are " + room.PlayerCount + " in room ");
    }
}