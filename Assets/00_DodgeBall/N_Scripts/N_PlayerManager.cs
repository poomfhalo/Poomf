﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class N_PlayerManager : MonoBehaviour
{
    List<SpawnPoint> playerSpawnPoints = new List<SpawnPoint>();
    PhotonView view = null;

    void Start()
    {
        view = GetComponent<PhotonView>();
        SpawnPC();
    }

    void SpawnPC()
    {
        playerSpawnPoints = FindObjectsOfType<SpawnPoint>().ToList();
        int i = UnityEngine.Random.Range(0, playerSpawnPoints.Count);
        SpawnPoint s = playerSpawnPoints[i];
        GameObject g = N_GameManager.MakeObj(N_Prefab.Player, s.position, s.rotation);
        g.transform.SetParent(transform);
        N_PC player = g.GetComponent<N_PC>();
        player.Initialize(view.Controller);
        Debug.Log(view.Controller + " Created a PC ", g);
    }
}
