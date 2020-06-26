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
        ball.E_OnGroundedAfterTime += () => {
            Log.Warning("Enabled Syncer, from grounded");
            syncer.enabled = true;
        };
        ball.reflection.onReflected += () => {
            Log.Warning("Enabled Syncer, from reflected");
            syncer.enabled = true;
        };

        ball.launchTo.onLaunchedTo += () => {
            Log.Warning("disabled Syncer, from onLaunchedTo");
            syncer.enabled = false;
        };
        ball.goTo.onGoto += () =>{
            Log.Warning("disabled Syncer, from onGoTo");
            syncer.enabled = false;
        };
        ball.launchUp.onLaunchedUp += () => {
            Log.Warning("disabled Syncer, from onLaunchedUp");
            syncer.enabled = false;
        };
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
        ball.ExtCanDetectGroundByTrig = () => PhotonNetwork.IsMasterClient;
        ball.reflection.extReflectionTest = PhotonNetwork.IsMasterClient;
    }
}