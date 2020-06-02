using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PC))]
public class N_PC : MonoBehaviour
{
    PhotonView photonView = null;
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if(photonView && !photonView.IsMine)
        {
            GetComponent<PC>().enabled = false;
        }
    }
}