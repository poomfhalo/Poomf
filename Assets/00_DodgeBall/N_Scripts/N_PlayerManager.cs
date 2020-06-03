using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class N_PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject pc = null;
    List<SpawnPoint> playerSpawnPoints = new List<SpawnPoint>();

    public override void OnEnable()
    {
        base.OnEnable();
        N_TeamsManager.instance.onTeamsAreSynced += OnTeamsAreSynced;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        N_TeamsManager.instance.onTeamsAreSynced -= OnTeamsAreSynced;
    }

    IEnumerator Start()
    {
        if (photonView.IsMine)
        {
            yield return StartCoroutine(SpawnPC());
        }
    }
    IEnumerator SpawnPC()
    {
        yield return 0;
        pc = N_Extentions.N_MakeObj(N_Prefab.Player, Vector3.zero,Quaternion.identity);
        pc.GetComponent<PhotonView>().RPC("Initialize", RpcTarget.All, GetComponent<PhotonView>().ViewID);
        yield return new WaitForSeconds(0.1f);
        Debug.Log(photonView.Controller + " Created a PC ", pc);
        PhotonNetwork.RaiseEvent(N_GameManager.OnCreatedPC, null, N_GameManager.GetDefOps, SendOptions.SendReliable);
    }

    private SpawnPoint SetUpPosition()
    {
        //N_TeamsManager.GetTeam();
        playerSpawnPoints = FindObjectsOfType<SpawnPoint>().ToList();
        //playerSpawnPoints.FindAll(p=>p.BelongsTo == )
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

        return s;
    }

    private void OnTeamsAreSynced()
    {
        if (pc == null)
            pc = N_Extentions.FindNetworkedObj<N_PC>().gameObject;
    }
}
