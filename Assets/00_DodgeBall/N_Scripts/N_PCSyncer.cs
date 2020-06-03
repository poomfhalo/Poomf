using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class N_PCSyncer : N_PC, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
