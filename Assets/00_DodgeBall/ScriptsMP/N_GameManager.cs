using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using System.Linq;

public enum N_Prefab { Player }

[Serializable]
public class LoadablePrefab
{
    public N_Prefab type = N_Prefab.Player;
    public string name = "N_Player";
    public LoadablePrefab(){ }
    public LoadablePrefab(N_Prefab type, string name)
    {
        this.type = type;
        this.name = name;
    }
}

public class N_GameManager : N_Singleton<N_GameManager>
{
    [SerializeField] List<LoadablePrefab> prefabs = new List<LoadablePrefab> { new LoadablePrefab(N_Prefab.Player,"N_Player") };

    List<SpawnPoint> playerSpawnPoints = new List<SpawnPoint>();
    void Start()
    {
        playerSpawnPoints = FindObjectsOfType<SpawnPoint>().ToList();
        int i = UnityEngine.Random.Range(0, playerSpawnPoints.Count);
        SpawnPoint s = playerSpawnPoints[i];
        MakeObj(N_Prefab.Player, s.position, s.rotation);
    }

    public static GameObject MakeObj(N_Prefab prefab,Vector3 pos,Quaternion rot, byte group = 0, object[] data = null)
    {
        LoadablePrefab p = instance.prefabs.Single(f => f.type == prefab);
        GameObject o = PhotonNetwork.Instantiate(p.name, pos, rot, group, data);
        return o;
    }
}