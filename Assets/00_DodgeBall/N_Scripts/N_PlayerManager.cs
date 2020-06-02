using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class N_PlayerManager : MonoBehaviourPunCallbacks
{
    List<SpawnPoint> playerSpawnPoints = new List<SpawnPoint>();

    void Start()
    {
        SpawnPC();
    }

    void SpawnPC()
    {
        playerSpawnPoints = FindObjectsOfType<SpawnPoint>().ToList();
        SpawnPoint s = playerSpawnPoints.Find(p => p.CheckPlayer(photonView.Controller.ActorNumber));
        if (s == null)
        {
            int maxTries = 60;
            do
            {
                int i = UnityEngine.Random.Range(0, playerSpawnPoints.Count);
                s = playerSpawnPoints[i];
                maxTries = maxTries - 1;
                if (maxTries <= 0)
                    break;
            } while (s == null || s.CheckPlayer(photonView.Controller.ActorNumber));
            s.GetComponent<PhotonView>().RPC("Fill", RpcTarget.All, photonView.Controller.ActorNumber);
        }

        GameObject g = N_GameManager.MakeObj(N_Prefab.Player, s.position, s.rotation);
        g.transform.SetParent(transform);
        N_PC player = g.GetComponent<N_PC>();
        player.Initialize(photonView.Controller);
        Debug.Log(photonView.Controller + " Created a PC ", g);
    }
}
