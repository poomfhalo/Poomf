using UnityEngine;
using Photon.Pun;

//Responsible for delegating, whether we have the button held or not.
//responsible for delegating, reciption call to local player.
public class N_BallReciever : MonoBehaviour
{
    PhotonView pv = null;
    BallReciever reciever = null;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        reciever = GetComponent<BallReciever>();
        reciever.extCanRecieveBall = PhotonNetwork.IsMasterClient;
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
        Debug.LogWarning("Recieved on Master, Calling To Recieved, on Client");
        reciever.RecieveBall();
    }
}