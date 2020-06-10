using UnityEngine;
using Photon.Pun;
using GW_Lib;

[RequireComponent(typeof(Dodgeball))]
public class N_Dodgeball : N_Singleton<N_Dodgeball>, IPunObservable
{
    [SerializeField] float catchUpSpeed = 0;
    [Header("Read Only")]
    [SerializeField] Vector3 netPos = new Vector3();

    Rigidbody rb3d = null;
    PhotonView pv = null;
    Dodgeball ball = null;
    float lastLag = 0;
    Vector3 netVel = Vector3.zero;
    bool firstRead = true;

    public override void OnEnable()
    {
        base.OnEnable();
        pv = GetComponent<PhotonView>();
        ball = GetComponent<Dodgeball>();
        rb3d = GetComponent<Rigidbody>();
        ball.OnCommandActivated += SendCommand;
    }
    void Start()
    {
        netPos = rb3d.position;
        ball.launchTo.ApplyActionWithCommand = () => false;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        ball.OnCommandActivated -= SendCommand;
        firstRead = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb3d.position);

            stream.SendNext(rb3d.velocity);
            stream.SendNext((int)ball.ballState);
        }
        else if (stream.IsReading)
        {
            if(firstRead)
            {
                firstRead = false;
                return;
            }

            netPos = (Vector3)stream.ReceiveNext();
            netVel = (Vector3)stream.ReceiveNext();
            ball.ballState = (Dodgeball.BallState)(int)stream.ReceiveNext();
        }
        lastLag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
        netPos = netPos + netVel * lastLag;
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
            return;
        if (ball.IsHeld || ball.IsGoingToChara)
            return;

        ball.SetKinematic(true);
        Vector3 targetPos = Vector3.Lerp(rb3d.position, netPos, catchUpSpeed * Time.fixedDeltaTime);
        rb3d.MovePosition(targetPos);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawSphere(netPos,GetComponent<SphereCollider>().radius);
    }

    private void SendCommand(DodgeballCommand command)
    {
        int holder = -1;
        if (ball.GetHolder())
        {
            if(!ball.GetHolder().GetComponent<PhotonView>().IsMine)//Only Send commands if the holder is the local player
            {
                return;
            }
            holder = ball.GetHolder().GetComponent<N_PC>().ActorID;
        }

        if(command == DodgeballCommand.LaunchUp || command == DodgeballCommand.GoToChara)
        {
            Debug.Log("N_Ball() :: sending command " + command + " :: is not allowed");
            return;
        }

        if (command  == DodgeballCommand.LaunchTo)
        {
            pv.RPC("RecieveCommand", RpcTarget.AllViaServer, (int)command, holder, ball.lastAppliedThrow, ball.lastTargetPos);
        }
        else
        {
            pv.RPC("RecieveCommand", RpcTarget.Others, (int)command, holder, ball.lastAppliedThrow, ball.lastTargetPos);
        }
        Debug.Log("N_Ball().SendCommand :: " + command, ball.GetHolder());
    }

    [PunRPC]
    private void RecieveCommand(int cmd,int holder,byte lastAppliedThrow,Vector3 lastTargetPos)
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
                    n_holder.GetComponent<BallGrabber>().GrabBall();
                break;
            case DodgeballCommand.LaunchTo:
                BallThrowData d = DodgeballGameManager.GetThrow(lastAppliedThrow);
                ball.launchTo.GoLaunchTo(lastTargetPos, d);
                break;
        }
    }

}