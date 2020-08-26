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
        if (!transform.parent)
            return;

        ball = transform.parent;
        rb3d = ball.GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
        i.Enable();
        i.performed += InputCalled;
    }
    private void InputCalled(InputAction.CallbackContext o)
    {
        if(o.performed)
        {
            sendingData = !sendingData;
            Log.Warning("Flipped Reading Data State");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        string s = "Sent Server Time " + info.SentServerTime.ToString() + "\n";
        var v = FindObjectOfType<CustomDebugText>();


        double newTime = info.SentServerTime;
        double diff = System.Math.Abs(newTime - lastRecordedTime);
        bool reachedTimeThreshold = diff > recordFrequency;

        if (reachedTimeThreshold)
            lastRecordedTime = newTime;

        if (stream.IsWriting)
        {
            Debug.LogWarning("WTF");
            if (!reachedTimeThreshold || !sendingData)
                return;

            Debug.LogWarning("Is Writing");
            stream.SendNext(ball.position);
            stream.SendNext(rb3d.velocity);
            v.AssignText(s);
        }
        else if (stream.IsReading)
        {
            Debug.LogWarning("Trying to read");
            if (stream.Count == 0)
                return;

            Debug.LogWarning("IsReading");

            Vector3 position = (Vector3)stream.ReceiveNext();
            Vector3 velocity = (Vector3)stream.ReceiveNext();

            if(positions.Count>maxDataSlots)
            {
                positions.RemoveAt(0);
                velocities.RemoveAt(0);
                Debug.LogWarning("removed Soemthing?");
            }
            Debug.LogWarning("ADded Element");

            positions.Add(position);
            velocities.Add(velocity);

            s = positions.Count.ToString();
            v.AssignText(s);
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
