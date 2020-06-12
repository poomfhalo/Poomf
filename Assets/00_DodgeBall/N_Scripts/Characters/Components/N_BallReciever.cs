using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

    }
    void OnDisable()
    {
        if (!pv.IsMine)
            return;

    }


}