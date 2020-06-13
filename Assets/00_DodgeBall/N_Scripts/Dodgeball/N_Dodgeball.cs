using UnityEngine;
using Photon.Pun;
using GW_Lib;
using System.Linq;
using System;

[RequireComponent(typeof(Dodgeball))]
public class N_Dodgeball : N_Singleton<N_Dodgeball>, IPunObservable
{
    public float lastLag { private set; get; }

    [SerializeField] float catchUpSpeed = 5;
    [SerializeField] float arrivalDist = 0.35f;

    [Header("Read Only")]
    [SerializeField] Vector3 netPos = new Vector3();
    [SerializeField] Vector3 netVel = Vector3.zero;

    Rigidbody rb3d = null;
    PhotonView pv = null;
    Dodgeball ball = null;
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
        if(!pv.IsMine)
            netPos = rb3d.position;
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
            stream.SendNext(rb3d.position);
            stream.SendNext(rb3d.velocity);
        }
        else if (stream.IsReading)
        {
            if (firstRead)
            {
                firstRead = false;
                return;
            }

            netPos = (Vector3)stream.ReceiveNext();
            netVel = (Vector3)stream.ReceiveNext();
        }
        lastLag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
        netPos = netPos + netVel * lastLag;
    }
    void FixedUpdate()
    {
        if (photonView.IsMine)
            return;
        if (ball.IsHeld || ball.IsFlying)
            return;

        ball.SetKinematic(false);
        Vector3 posDir = (netPos - rb3d.position).normalized;
        Vector3 posVel = posDir * catchUpSpeed;
        Vector3 targetVel = netVel + posVel;
        Vector3 smoothVel = Vector3.Slerp(rb3d.velocity, targetVel, lastLag * Time.fixedDeltaTime * catchUpSpeed);

        float dist = Vector3.Distance(rb3d.position, netPos);
        if(dist<=arrivalDist)
        {
            float pDist = dist / arrivalDist;
            smoothVel = smoothVel * pDist;
        }
        rb3d.velocity = smoothVel;

        //ball.SetKinematic(true);
        //Vector3 targetPos = Vector3.Lerp(rb3d.position, netPos, catchUpSpeed * Time.fixedDeltaTime);
        //rb3d.MovePosition(targetPos);
    }
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        if (GetComponent<PhotonView>().IsMine)
            return;
        Gizmos.color = Color.grey;
        Gizmos.DrawSphere(netPos, GetComponent<SphereCollider>().radius);
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