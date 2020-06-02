using System.Collections;
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
        view.RPC("SpawnPC", view.Controller,new object[1] { 5 });
    }

    [PunRPC]
    void SpawnPC(int someNum)
    {
        playerSpawnPoints = FindObjectsOfType<SpawnPoint>().ToList();
        int i = UnityEngine.Random.Range(0, playerSpawnPoints.Count);
        SpawnPoint s = playerSpawnPoints[i];
        GameObject g = N_GameManager.MakeObj(N_Prefab.Player, s.position, s.rotation);
        Debug.Log("Created PC " + someNum, g);
    }
}
