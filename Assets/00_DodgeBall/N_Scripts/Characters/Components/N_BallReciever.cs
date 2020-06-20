using UnityEngine;
using Photon.Pun;

public class N_BallReciever : MonoBehaviour,IPunObservable
{
    PhotonView pv = null;
    BallReciever reciever = null;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        reciever = GetComponent<BallReciever>();
        if(!PhotonNetwork.IsMasterClient)
        {
            reciever.extCanRecieveBall = false;
        }
    }
    void OnEnable()
    {
        if (pv.IsMine && !PhotonNetwork.IsMasterClient)
        {
            reciever.onRecievedButtonInput += OnRecievedButtonInput;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            reciever.onBallGrabbed += OnBallGrabbed;
        }
    }
    void OnDisable()
    {
        if (pv.IsMine && !PhotonNetwork.IsMasterClient)
        {
            reciever.onRecievedButtonInput -= OnRecievedButtonInput;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            reciever.onBallGrabbed -= OnBallGrabbed;
        }
    }

    private void OnRecievedButtonInput(bool state)
    {
        pv.RPC("UpdateButtonClick", PhotonNetwork.MasterClient, state);
    }
    private void OnBallGrabbed()
    {
        pv.RPC("R_UpdateBallState", RpcTarget.Others);
    }

    [PunRPC]
    private void UpdateButtonClick(bool state) => reciever.RecieveButtonInput(state);
    [PunRPC]
    private void R_UpdateBallState()
    {
        Debug.LogWarning("Grabbed on Master, Calling To Grab, on Client");
        reciever.RecieveBall();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(reciever.isButtonClicked);
        }
        else if(stream.IsReading)
        {
            bool pressState = (bool)stream.ReceiveNext();
            UpdateButtonClick(pressState);
        }
    }
}