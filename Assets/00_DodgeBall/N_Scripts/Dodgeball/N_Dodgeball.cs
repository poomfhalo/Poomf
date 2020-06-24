using UnityEngine;
using Photon.Pun;
using Smooth;
using System.Collections;

//Responsible for enabling/Disabling Syncer, and syncronizing the BallState.
[RequireComponent(typeof(Dodgeball))]
public class N_Dodgeball : N_Singleton<N_Dodgeball>, IPunObservable
{
    Dodgeball ball = null;
    SmoothSyncPUN2 syncer = null;

    public override void OnEnable()
    {
        base.OnEnable();
        ball = GetComponent<Dodgeball>();
        syncer = GetComponent<SmoothSyncPUN2>();

        StartCoroutine(NetworkSetup());
    }
    void Start()
    {
        GetComponent<DodgeballLaunchUp>().onLaunchedUp += () => syncer.enabled = false;
        ball.E_OnGroundedAfterTime += () => syncer.enabled = true;
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
        ball.CanDetectGroundByTrig = () => PhotonNetwork.IsMasterClient;
        ball.reflection.extReflectionTest = PhotonNetwork.IsMasterClient;
    }
}