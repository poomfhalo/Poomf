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

        netPos = rb3d.position;
        ball.OnCommandActivated += SendCommand;
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
            stream.SendNext(rb3d.position.x);
            stream.SendNext(rb3d.position.y);
            stream.SendNext(rb3d.position.z);

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

            netPos.x = (float)stream.ReceiveNext();
            netPos.y = (float)stream.ReceiveNext();
            netPos.z = (float)stream.ReceiveNext();
            netVel = (Vector3)stream.ReceiveNext();
            ball.ballState = (Dodgeball.BallState)(int)stream.ReceiveNext();
        }
        lastLag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
        netPos = netPos + netVel * lastLag;
        //rb3d.MovePosition(rb3d.position + rb3d.velocity * lastLag);
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
                if (!n_holder.HasBall)
                    n_holder.GetComponent<BallCatcher>().StartCatchAction();
                break;
        }
    }


}