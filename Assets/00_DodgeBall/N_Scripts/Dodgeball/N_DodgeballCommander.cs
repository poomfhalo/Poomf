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
    void OnEnable()
    {
        ball.OnCommandActivated += SendCommand;
    }
    void OnDisable()
    {
        ball.OnCommandActivated -= SendCommand;
    }

    private void SendCommand(DodgeballCommand command)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        int holder = -1;
        if (ball.GetHolder())
        {
            if (!ball.GetHolder().GetComponent<PhotonView>().IsMine)//Only Send commands if the command caller is the local player
            {
                return;
            }
            holder = ball.GetHolder().GetComponent<N_PC>().ActorID;
        }

        if (command == DodgeballCommand.LaunchUp)
        {
            Debug.Log("N_Ball() :: sending command " + command + " :: is not allowed");
            return;
        }

        this.InvokeDelayed(lastLag * 2, () => {
            if (command == DodgeballCommand.LaunchTo)
            {
                pv.RPC("RecieveCommand", RpcTarget.AllViaServer, (int)command, holder, ball.lastAppliedThrow, ball.lastTargetPos);
            }
            else
            {
                pv.RPC("RecieveCommand", RpcTarget.Others, (int)command, holder, ball.lastAppliedThrow, ball.lastTargetPos);
            }
            Debug.Log("N_Ball().SendCommand :: " + command, ball.GetHolder());
        });
    }

    [PunRPC]
    private void RecieveCommand(int cmd, int holder, byte lastAppliedThrow, Vector3 lastTargetPos)
    {
        DodgeballCommand command = (DodgeballCommand)cmd;
        Debug.Log("N_Ball().RecieveCommand :: " + command);
        DodgeballCharacter n_holder = null;
        if (holder != -1)
        {
            n_holder = N_TeamsManager.GetPlayer(holder).GetComponent<DodgeballCharacter>();
        }
        switch (command)
        {
            case DodgeballCommand.GoToChara:
                if (n_holder && !n_holder.HasBall && !ball.IsGoingToChara)
                {
                    Debug.Log("called from here?");
                    n_holder.GetComponent<BallGrabber>().GrabBall();
                }
                break;
            case DodgeballCommand.LaunchTo:
                BallThrowData d = DodgeballGameManager.GetThrow(lastAppliedThrow);
                ball.launchTo.GoLaunchTo(lastTargetPos, d);
                break;
        }
    }
}