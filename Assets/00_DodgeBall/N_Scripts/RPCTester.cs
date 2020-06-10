using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PhotonView))]
public class RPCTester : MonoBehaviour
{
    [SerializeField] InputAction a = null;
    PhotonView pv = null;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        a.started += OnStarted;
    }

    private void OnStarted(InputAction.CallbackContext obj)
    {
        Debug.Log("Sending Command ");
        pv.RPC("RecievingCommand", RpcTarget.All);
    }
    [PunRPC]
    void RecievingCommand()
    {
        Debug.Log("Have Recieved Command");
    }
    void Update()
    {

    }
}