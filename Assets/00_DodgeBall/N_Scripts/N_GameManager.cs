using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using System.Linq;

public enum N_Prefab { PlayerManager,Player }

public class N_GameManager : N_Singleton<N_GameManager>
{
    public List<LoadablePrefab> Prefabs => prefabs;
    [SerializeField] List<LoadablePrefab> prefabs = new List<LoadablePrefab> { new LoadablePrefab(N_Prefab.Player,"N_PlayerManager") };

    void Reset()
    {
        prefabs = new List<LoadablePrefab>();
        foreach (var e in Enum.GetValues(typeof(N_Prefab)).Cast<N_Prefab>())
        {
            prefabs.Add(new LoadablePrefab(e,"N_" + e.ToString()));
        }
    }
    protected override void Awake()
    {
        base.Awake();
        N_Extentions.prefabs = prefabs;
    }

    [PunRPC]
    private void CreatePlayerManager()
    {
        GameObject g = N_Extentions.N_MakeObj(N_Prefab.PlayerManager, Vector3.zero, Quaternion.identity);
        Debug.Log("Created PlayerManager " + PhotonNetwork.LocalPlayer.NickName, g);
    }

    public void CreatePlayers()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        N_TeamsManager teams = GetComponent<N_TeamsManager>();
        var players = PhotonNetwork.PlayerList;
        foreach (var p in players)
        {
            GameObject g = N_Extentions.N_MakeObj(N_Prefab.PlayerManager, Vector3.zero, Quaternion.identity);
            g.GetComponent<PhotonView>().TransferOwnership(p);

            if (p.ActorNumber % 2 == 0)
            {
                Debug.Log(p.NickName + " Joined team A");
                teams.AddPlayer(TeamTag.A, p.ActorNumber);
            }
            else
            {
                Debug.Log(p.NickName + " Joined team B");
                teams.AddPlayer(TeamTag.B, p.ActorNumber);
            }
        }
    }

}