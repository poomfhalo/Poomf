﻿using UnityEngine;
using Photon.Pun;
using Smooth;
using System.Collections;

[RequireComponent(typeof(Dodgeball))]
public class N_Dodgeball : N_Singleton<N_Dodgeball>, IPunObservable
{
    PhotonView pv = null;
    Dodgeball ball = null;
    SmoothSyncPUN2 syncer = null;

    public override void OnEnable()
    {
        base.OnEnable();
        pv = GetComponent<PhotonView>();
        ball = GetComponent<Dodgeball>();
        syncer = GetComponent<SmoothSyncPUN2>();

        GetComponent<DodgeballLaunchUp>().onLaunchedUp += OnLaunchedUp;
        ball.E_OnGroundedAfterTime += OnGrounded;

        StartCoroutine(NetworkSetup());
    }
    public override void OnDisable()
    {
        base.OnDisable();
        GetComponent<DodgeballLaunchUp>().onLaunchedUp -= OnLaunchedUp;
        ball.E_OnGroundedAfterTime -= OnGrounded;
    }
    void FixedUpdate()
    {
        if (ball.IsHeld || ball.IsFlying)
        {
            if (syncer.enabled)
            {
                syncer.enabled = false;
            }
            return;
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(ball.ballState);
        }
        else if (stream.IsReading)
        {
            int s = (int)stream.ReceiveNext();
            Dodgeball.BallState state = (Dodgeball.BallState)s;
            ball.ballState = state;
        }
    }

    private IEnumerator NetworkSetup()
    {
        while (!PhotonNetwork.IsConnected)
        {
            yield return 0;
        }
        ball.CanApplyOnGroundHit = () => PhotonNetwork.IsMasterClient;
    }

    private void OnLaunchedUp()
    {
        syncer.enabled = false;
    }
    private void OnGrounded()
    {
        syncer.enabled = true;
    }
}