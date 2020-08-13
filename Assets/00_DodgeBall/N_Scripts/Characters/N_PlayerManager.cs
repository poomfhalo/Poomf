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
        name = "Manager " + photonView.Controller.NickName;
        if (photonView.IsMine)
        {
            yield return StartCoroutine(SpawnPC());
        }
    }
    IEnumerator SpawnPC()
    {
        yield return 0;
        pc = N_Extentions.N_MakeObj(N_Prefab.Player, Vector3.zero, Quaternion.identity);
        pc.GetComponent<PhotonView>().RPC("OnCreated", RpcTarget.All, GetComponent<PhotonView>().ViewID);
        yield return new WaitForSeconds(0.1f);
        Log.LogL0(photonView.Controller + " Created a PC ", pc);

        N_Extentions.N_RaiseEvent(N_GameManager.N_OnCreatedPC, null, false);
    }

    private void OnTeamsAreSynced()
    {
        if (pc == null)
        {
            pc = N_Extentions.GetCharacter(GetComponent<PhotonView>().Controller.ActorNumber).gameObject;

            PlayersRunDataSO dataSO = PlayersRunDataSO.Instance;
            PlayerRunData data = dataSO.GetPlayerRunData(photonView.ControllerActorNr);
            CharaSkinData skinData = data.charaSkinData.CreateSkinData();
            pc.GetComponentInChildren<CustomizablePlayer>().SetNewSkinData(skinData);

            Log.Message("Applying skin data " + skinData + " to " + data.charaName);
        }

        if (!photonView.IsMine)
            return;

        Team team = TeamsManager.GetTeam(pc.GetComponent<DodgeballCharacter>());
        if(team == null)
        {
            Log.Error("Failed To Find Team For " + name);
            return;
        }

        var p = FindObjectsOfType<TaggedSpawnPoint>().ToList().Find(s => s.HasTag("MainCamera") && s.BelongsTo(team.teamTag));
        p.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 15;
    }
}