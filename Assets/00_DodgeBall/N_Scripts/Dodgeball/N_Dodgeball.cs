﻿using UnityEngine;
using Photon.Pun;
using GW_Lib;
using System.Linq;
using System;

[RequireComponent(typeof(Dodgeball))]
public class N_Dodgeball : N_Singleton<N_Dodgeball>, IPunObservable
{
    public float lastLag { private set; get; }

    [SerializeField] float catchUpSpeed = 0;
    [Header("Read Only")]
    //[SerializeField] Vector3 netPos = new Vector3();

    Rigidbody rb3d = null;
    PhotonView pv = null;
    Dodgeball ball = null;
    Vector3 netVel = Vector3.zero;
    bool firstRead = true;

    public override void OnEnable()
    {
        base.OnEnable();
        pv = GetComponent<PhotonView>();
        ball = GetComponent<Dodgeball>();
        rb3d = GetComponent<Rigidbody>();
    }
    void Start()
    {
        //netPos = rb3d.position;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        firstRead = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(rb3d.position);
            //stream.SendNext(rb3d.velocity);
            stream.SendNext((int)ball.ballState);
        }
        else if (stream.IsReading)
        {
            if (firstRead)
            {
                firstRead = false;
                return;
            }

            //netPos = (Vector3)stream.ReceiveNext();
            //netVel = (Vector3)stream.ReceiveNext();
            ball.ballState = (Dodgeball.BallState)(int)stream.ReceiveNext();
        }
        lastLag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
        //netPos = netPos + netVel * lastLag;
    }
    void FixedUpdate()
    {
        if (photonView.IsMine)
            return;
        if (ball.IsHeld || ball.IsHeld)
            return;

        //ball.SetKinematic(true);
        //Vector3 targetPos = Vector3.Lerp(rb3d.position, netPos, catchUpSpeed * Time.fixedDeltaTime);
        //rb3d.MovePosition(targetPos);
    }
    void OnDrawGizmos()
    {
        //Gizmos.color = Color.grey;
        //Gizmos.DrawSphere(netPos, GetComponent<SphereCollider>().radius);
    }

    public int GetHolder()
    {
        int holder = -1;
        if (ball.GetHolder())//Only Send commands if the command caller is the local player
        {
            if (!ball.GetHolder().GetComponent<PhotonView>().IsMine)
            {
                return holder;
            }
            holder = ball.GetHolder().GetComponent<N_PC>().ActorID;
        }
        return holder;
    }
}