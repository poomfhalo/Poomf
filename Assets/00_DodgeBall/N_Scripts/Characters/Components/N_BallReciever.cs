using UnityEngine;
using Photon.Pun;

public class N_BallReciever : MonoBehaviour
{
    PhotonView pv = null;
    BallReciever reciever = null;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        reciever = GetComponent<BallReciever>();
    }
    void OnEnable()
    {
        if (pv.IsMine)
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
        if (pv.IsMine)
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
        pv.RPC("UpdateButtonClick", RpcTarget.Others, state);
    }
    private void OnBallGrabbed()
    {
        pv.RPC("R_UpdateBallState", RpcTarget.Others);
    }

    [PunRPC]
    private void UpdateButtonClick(bool state) => reciever.RecieveButtonInput(state);
    [PunRPC]
    private void R_UpdateBallState() => reciever.TryGrabBall();
}