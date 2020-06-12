﻿using UnityEngine;
using Photon.Pun;
using System;

public class N_BallReciever : MonoBehaviour
{
    PhotonView pv = null;
    BallReciever reciever = null;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    void OnEnable()
    {
        if (!pv.IsMine)
            return;
        reciever.onRecievedButtonInput += OnRecievedButtonInput;
    }
    void OnDisable()
    {
        if (!pv.IsMine)
            return;
        reciever.onRecievedButtonInput -= OnRecievedButtonInput;
    }
    private void OnRecievedButtonInput(bool state)
    {
        pv.RPC("UpdateButtonClick", RpcTarget.Others, state);
    }

    [PunRPC]
    private void UpdateButtonClick(bool state) => reciever.RecieveButtonInput(state);
}