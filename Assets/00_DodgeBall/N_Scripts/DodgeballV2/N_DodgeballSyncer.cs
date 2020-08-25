using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class N_DodgeballSyncer : MonoBehaviour, IPunObservable
{
    [Tooltip("This Means, we record at least one result ever recordSpeed in seconds")]
    [SerializeField] double recordSpeed = 0.1;
    [SerializeField] int maxDataSlots = 600;

    [Header("Read Only")]
    [SerializeField] float recordCounter = 0;
    [SerializeField] bool sendingData = false;
    [SerializeField] double lastRecordedTime = 0;
    [SerializeField] List<Vector3> positions = new List<Vector3>();
    [SerializeField] List<Vector3> velocities = new List<Vector3>();

    Transform ball = null;
    Rigidbody rb3d = null;
    PhotonView pv = null;

    void Start()
    {
        ball = transform.parent;
        rb3d = ball.GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(!enabled)
        {
            Debug.Log("not enabled? Returning");
            return;
        }
        string s = "Sent Server Time " + info.SentServerTime.ToString() + "\n";
        FindObjectOfType<CustomDebugText>().AssignText(s);

        double newTime = info.SentServerTime;
        double diff = System.Math.Abs(newTime - lastRecordedTime);
        bool allowWriting = diff > recordSpeed;

        if(allowWriting)
            lastRecordedTime = newTime;

        if (stream.IsWriting)
        {
            print("WTF");
            if (!allowWriting || !sendingData)
                return;

            print("Is Writing");
            stream.SendNext(ball.position);
            stream.SendNext(rb3d.velocity);
        }
        else if (stream.IsReading)
        {
            print("TRing to read");
            if (stream.Count == 0)
                return;

            print("IsREading");
            Vector3 position = (Vector3)stream.ReceiveNext();
            Vector3 velocity = (Vector3)stream.ReceiveNext();

            if(positions.Count>maxDataSlots)
            {
                positions.RemoveAt(0);
                velocities.RemoveAt(0);
                Debug.Log("removed Soemthing?");
            }
            Debug.Log("ADded Element");

            positions.Add(position);
            velocities.Add(velocity);
        }
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
