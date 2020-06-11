using System.Collections;
using System.Collections.Generic;
using GW_Lib;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(N_Dodgeball))]
public class N_DodgeballCommander : MonoBehaviour
{
    N_Dodgeball n_ball = null;
    Dodgeball ball = null;
    PhotonView pv = null;
    float lastLag => n_ball.lastLag;

    void Awake()
    {
        n_ball = GetComponent<N_Dodgeball>();
        ball = GetComponent<Dodgeball>();
        pv = GetComponent<PhotonView>();
    }

    void OnEnable()=> ball.OnCommandActivated += SendCommand;
    void OnDisable()=> ball.OnCommandActivated -= SendCommand;

    private void SendCommand(DodgeballCommand command)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (command == DodgeballCommand.LaunchUp)
        {
            Debug.Log("N_Ball() :: sending command " + command + " :: is not allowed");
            return;
        }

        int sender = pv.Controller.ActorNumber;

        this.InvokeDelayed(lastLag * 2, () => {
            Debug.Log("N_Ball().SendCommand :: " + command, ball.GetHolder());
            switch (command)
            {
                case DodgeballCommand.GoToChara:
                    pv.RPC("R_GoToChara", RpcTarget.Others, n_ball.GetHolder());
                    break;
                case DodgeballCommand.LaunchTo:
                    pv.RPC("R_LaunchTo", RpcTarget.Others, ball.lastAppliedThrow, ball.lastTargetPos);
                    break;
            }
        });
    }

    [PunRPC]
    private void R_GoToChara(int holder)
    {
        Debug.Log("Commander :: R_GoToChara()");
        DodgeballCharacter n_holder = null;
        if (holder != -1)
        {
            n_holder = N_TeamsManager.GetPlayer(holder).GetComponent<DodgeballCharacter>();
        }

        if (n_holder && !n_holder.HasBall && !ball.IsHeld)
        {
            Debug.Log("Called Grab From Here");
            n_holder.GetComponent<BallGrabber>().GrabBall();
        }
    }
    [PunRPC]
    private void R_LaunchTo(byte lastAppliedThrow,Vector3 lastTargetPos)
    {
        Debug.Log("Commander :: R_LaunchTo()");
        if (ball.ballState == Dodgeball.BallState.Flying)
            return;

        Debug.Log("Launched Ball From Here");
        BallThrowData d = DodgeballGameManager.GetThrow(lastAppliedThrow);
        ball.launchTo.GoLaunchTo(lastTargetPos, d);
    }
}