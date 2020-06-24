using GW_Lib;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(N_Dodgeball))]
public class N_DodgeballCommander : MonoBehaviour
{
    N_Dodgeball n_ball = null;
    Dodgeball ball = null;
    PhotonView pv = null;
    float lastLag => N_GameManager.instance.LastLag;

    void Awake()
    {
        n_ball = GetComponent<N_Dodgeball>();
        ball = GetComponent<Dodgeball>();
        pv = GetComponent<PhotonView>();
    }
    void OnEnable() => ball.OnCommandActivated += SendCommand;
    void OnDisable() => ball.OnCommandActivated -= SendCommand;

    private void SendCommand(DodgeballCommand command)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (command == DodgeballCommand.LaunchUp)
        {
            Log.Message("N_Ball() :: sending command " + command + " :: is not allowed");
            return;
        }

        this.InvokeDelayed(lastLag * 2, () => {
            Log.Message("N_Ball().SendCommand :: " + command, ball.holder);
            switch (command)
            {
                case DodgeballCommand.GoToChara:
                    pv.RPC("R_GoToChara", RpcTarget.Others, GetHolder());
                    break;
                case DodgeballCommand.LaunchTo:
                    pv.RPC("R_LaunchTo", RpcTarget.Others, ball.launchTo.lastAppliedThrow, ball.launchTo.lastTargetPos);
                    break;
            }
        });

        switch (command)
        {
            case DodgeballCommand.HitGround:
                pv.RPC("R_OnGroundHit", RpcTarget.Others);
                break;
            case DodgeballCommand.Reflection:
                DodgeballReflection reflection = ball.reflection;
                Vector3 reflectionVel = reflection.lastReflectionVel;
                Vector3 reflectionStartPoint = reflection.lastReflectionStartPoint;
                Vector3 reflectionTarget = reflection.lastReflectionTarget;
                int lastContact = reflection.lastContact.GetComponent<PhotonView>().Controller.ActorNumber;
                pv.RPC("R_Reflection", RpcTarget.Others, reflectionVel, reflectionStartPoint, reflectionTarget, lastContact);
                break;
        }
    }

    [PunRPC]
    private void R_GoToChara(int holder)
    {
        Log.Message("N_Ball().RPC :: R_GoToChara()");
        DodgeballCharacter n_holder = null;
        if (holder != -1)
        {
            n_holder = N_TeamsManager.GetPlayer(holder).GetComponent<DodgeballCharacter>();
        }

        if (n_holder && !n_holder.HasBall && !ball.IsHeld)
        {
            Debug.Log("Grabbed Ball By RPC");
            n_holder.GetComponent<BallGrabber>().GrabBall();
        }
    }
    [PunRPC]
    private void R_LaunchTo(byte lastAppliedThrow,Vector3 lastTargetPos)
    {
        Log.Message("N_Ball().RPC :: R_LaunchTo()");
        if (ball.ballState == Dodgeball.BallState.Flying)
            return;

        Log.Message("Launched Ball By RPC");
        BallThrowData d = DodgeballGameManager.GetThrow(lastAppliedThrow);
        ball.launchTo.GoLaunchTo(lastTargetPos, d);
    }
    [PunRPC]
    private void R_OnGroundHit()
    {
        Log.Message("N_Ball().RPC :: R_OnGroundHit");
        ball.OnGroundHit();
    }
    [PunRPC]
    private void R_Reflection(Vector3 reflectionVel, Vector3 reflectionStartPoint, Vector3 reflectionPoint,int lastContact)
    {
        DodgeballReflection reflection = ball.reflection;
        GameObject gLastContact = N_Extentions.GetCharacter(lastContact).gameObject;
        reflection.Reflect(reflectionVel, reflectionStartPoint, reflectionPoint,gLastContact);
        Log.Message("RPC_R_Reflection");
    }

    //Helper Functions
    public int GetHolder()
    {
        int holder = -1;
        if (ball.holder != null)//Only Send commands if the command caller is the local player
        {
            holder = ball.holder.GetComponent<N_PC>().ActorID;
        }
        return holder;
    }
}