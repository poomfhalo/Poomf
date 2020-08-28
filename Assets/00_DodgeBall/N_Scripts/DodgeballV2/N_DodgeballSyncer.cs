using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.InputSystem;
using System;

public class N_DodgeballSyncer : MonoBehaviourPunCallbacks, IPunObservable
{
    public InputAction i = null;
    [Tooltip("This Means, we record at least one result ever recordSpeed in seconds")]
    [SerializeField] double recordFrequency = 0.1;
    [SerializeField] int maxDataSlots = 600;
    [SerializeField] float satisfactionRad = 0.2f;

    [Header("Read Only")]
    [SerializeField] double lastRecordedTime = 0;
    [SerializeField] bool sendingData = false;
    [SerializeField] bool reachedTimeThreshold = false;
    [SerializeField] List<Vector3> positions = new List<Vector3>();
    [SerializeField] List<Vector3> velocities = new List<Vector3>();

    Transform ball = null;
    Rigidbody rb3d = null;
    PhotonView pv = null;
    Vector3 currVel = new Vector3();

    void Start()
    {
        if (!transform.parent)
            return;

        ball = transform.parent;
        rb3d = ball.GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
        i.Enable();
        i.performed += InputCalled;

        ball.GetComponent<DodgeballGoLaunchTo>().ApplyActionWithCommand = () => pv.IsMine;
    }
    void FixedUpdate()
    {
        if(positions.Count == 0)
            return;

        Vector3 dir = (positions[0] - rb3d.position).normalized;
        Vector3 vel = dir * velocities[0].magnitude;
        if (vel.magnitude<=0.2f)
            rb3d.MovePosition(positions[0]);
        else
        {
            Vector3 smoothPosition = rb3d.position + vel * Time.fixedDeltaTime;
            float distByVel = Vector3.Distance(rb3d.position, smoothPosition);
            float dist = Vector3.Distance(rb3d.position, positions[0]);
            if (dist < distByVel)
            {
                vel = vel.normalized * dist;
            }

            rb3d.MovePosition(smoothPosition);
        }

        if(Vector3.Distance(rb3d.position,positions[0])<=satisfactionRad)
        {
            positions.RemoveAt(0);
            velocities.RemoveAt(0);
        }
    }
    private void InputCalled(InputAction.CallbackContext o)
    {
        //if(o.performed)
        //{
        //    sendingData = !sendingData;
        //    Log.Warning("Flipped Reading Data State");
        //}
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        double newTime = info.SentServerTime;
        double diff = System.Math.Abs(newTime - lastRecordedTime);
        reachedTimeThreshold = diff > recordFrequency;

        if (reachedTimeThreshold)
            lastRecordedTime = newTime;

        if (stream.IsWriting)
        {
            if (!reachedTimeThreshold || !sendingData)
                return;

            stream.SendNext(ball.position);
            stream.SendNext(rb3d.velocity);
        }
        else if (stream.IsReading)
        {
            if (stream.Count == 0)
                return;

            Vector3 position = (Vector3)stream.ReceiveNext();
            Vector3 velocity = (Vector3)stream.ReceiveNext();

            if(positions.Count>maxDataSlots)
            {
                positions.RemoveAt(0);
                velocities.RemoveAt(0);
            }

            positions.Add(position);
            velocities.Add(velocity);

            var v = FindObjectOfType<CustomDebugText>();
            string s = positions.Count.ToString();
            v.AssignText(s);
        }
    }

    public void SendDataByRPC()
    {
        photonView.RPC("R_VelPos", RpcTarget.Others, rb3d.position, rb3d.velocity);
    }
    public void SendDataByRPC(int framesForceSend,float secondsDifference)
    { 
        IEnumerator ForceSendByInterval()
        {
            for (int i = 0; i < framesForceSend; i++)
            {
                SendDataByRPC();
                yield return new WaitForSeconds(secondsDifference);
            }
        }
        StartCoroutine(ForceSendByInterval());
    }

    [PunRPC]
    private void R_VelPos(Vector3 pos,Vector3 vel)
    {
        positions.Add(pos);
        velocities.Add(vel);
    }

    public void SetSendingData(bool toState)
    {
        sendingData = toState;
    }
    public void ClearData()
    {
        positions.Clear();
        velocities.Clear();
        SetSendingData(false);
    }
}
