﻿using System.Collections;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class N_PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject pc = null;

    public override void OnEnable()
    {
        base.OnEnable();
        N_GameManager.OnTeamsAreSynced += OnTeamsAreSynced;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        N_GameManager.OnTeamsAreSynced -= OnTeamsAreSynced;
    }

    IEnumerator Start()
    {
        if (photonView.IsMine)
        {
            yield return StartCoroutine(SpawnPC());
        }
        name = "Manager " + GetComponent<PhotonView>().Controller.NickName;
    }
    IEnumerator SpawnPC()
    {
        yield return 0;
        pc = N_Extentions.N_MakeObj(N_Prefab.Player, Vector3.zero, Quaternion.identity);
        pc.GetComponent<PhotonView>().RPC("OnCreated", RpcTarget.All, GetComponent<PhotonView>().ViewID);
        yield return new WaitForSeconds(0.1f);
        Debug.Log(photonView.Controller + " Created a PC ", pc);
        N_Extentions.N_RaiseEvent(N_GameManager.N_OnCreatedPC, null, false);
    }

    private void OnTeamsAreSynced()
    {
        if (pc == null)
        {
            pc = N_Extentions.GetCharacter(GetComponent<PhotonView>().Controller.ActorNumber).gameObject;
        }
        if (!photonView.IsMine)
            return;
        Team team = TeamsManager.GetTeam(pc.GetComponent<DodgeballCharacter>());
        if(team == null)
        {
            Log.Error("Failed To Find Team For " + name);
            return;
        }

        Camera cam = Camera.main;
        Debug.LogWarning(name + " Team : " + team.teamTag);
        var p = FindObjectsOfType<TaggedSpawnPoint>().ToList().Find(s => s.HasTag("MainCamera") && s.BelongsTo(team.teamTag));
        Debug.LogWarning(p.name);
        cam.transform.position = p.position;
        cam.transform.rotation = p.rotation;
    }
}
