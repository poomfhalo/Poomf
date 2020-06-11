using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using System;
using Photon.Pun;

[RequireComponent(typeof(CharaHitPoints))]
public class N_CharaHitPoints : MonoBehaviour
{
    CharaHitPoints hp = null;
    PhotonView pv = null;

    void Awake()
    {
        hp = GetComponent<CharaHitPoints>();
        pv = GetComponent<PhotonView>();
    }
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            hp.OnHPAction += OnHPAction;
        else
            hp.ApplyHealthChanges = () => false;
    }

    private void OnHPAction(HPAction action)
    {
        switch (action)
        {
            case HPAction.Subtract:
                pv.RPC("R_StartHitAction", RpcTarget.AllViaServer);
                break;
        }
    }
    private void R_StartHitAction()
    {

        hp.StartHitAction();
    }
}