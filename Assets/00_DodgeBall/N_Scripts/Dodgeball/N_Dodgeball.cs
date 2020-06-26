using UnityEngine;
using Photon.Pun;
using Smooth;
using System.Collections;

//Responsible for enabling/Disabling Syncer, and syncronizing the BallState.
[RequireComponent(typeof(Dodgeball))]
public class N_Dodgeball : N_Singleton<N_Dodgeball>,IPunObservable
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
        ball.E_OnStateUpdated += (s) => {
            switch (s)
            {
                case Dodgeball.BallState.OnGround:
                    syncer.enabled = true;
                    break;
                case Dodgeball.BallState.Held:
                    syncer.enabled = false;
                    break;
            }
        };
        ball.reflection.onReflected += () => {
            syncer.enabled = true;
        };

        ball.launchTo.onLaunchedTo += () => {
            syncer.enabled = false;
        };
        ball.goTo.onGoto += () =>{
            syncer.enabled = false;
        };
        ball.launchUp.onLaunchedUp += () => {
            syncer.enabled = false;
        };
    }
    private IEnumerator NetworkSetup()
    {
        while (!PhotonNetwork.IsConnected)
        {
            yield return 0;
        }
        ball.ExtCanDetectGroundByTrig = () => PhotonNetwork.IsMasterClient;
        ball.reflection.extReflectionTest = PhotonNetwork.IsMasterClient;
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
}