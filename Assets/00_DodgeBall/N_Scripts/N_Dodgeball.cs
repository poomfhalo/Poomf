using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

[RequireComponent(typeof(Dodgeball))]
public class N_Dodgeball : MonoBehaviour, IPunObservable
{
    [Header("Read Only")]
    [SerializeField] Vector3 networkedPos = new Vector3();

    Rigidbody rb3d = null;
    PhotonView pv = null;
    Dodgeball ball = null;

    void OnEnable()
    {
        pv = GetComponent<PhotonView>();
        ball = GetComponent<Dodgeball>();
        rb3d = GetComponent<Rigidbody>();

        ball.OnCommandActivated += SendCommand;
    }
    void Start()
    {

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(rb3d.position.x);
            stream.SendNext(rb3d.position.y);
            stream.SendNext(rb3d.position.z);
            stream.SendNext((int)ball.ballState);
        }
        else if(stream.IsReading)
        {
            networkedPos.x = (float)stream.ReceiveNext();
            networkedPos.y = (float)stream.ReceiveNext();
            networkedPos.z = (float)stream.ReceiveNext();
            ball.ballState = (Dodgeball.BallState)(int)stream.ReceiveNext();
        }
    }
    private void SendCommand(DodgeballCommand command)
    {
        int holder = ball.GetHolder().GetComponent<N_PC>().ActorID;
        pv.RPC("RecieveCommand", RpcTarget.Others, (int)command,holder);
    }

    [PunRPC]
    private void RecieveCommand(int c,int holder)
    {
        DodgeballCommand command = (DodgeballCommand)c;
        DodgeballCharacter n_holder = N_TeamsManager.GetPlayer(holder).GetComponent<DodgeballCharacter>();
        switch (command)
        {
            case DodgeballCommand.GoToChara:
                Debug.Log("Ball().GoingToChara Command RPC");
                //Dodgeball.GoTo(n_holder,);
                break;
        }
    }

}