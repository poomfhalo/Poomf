using UnityEngine;
using Photon.Pun;

public class N_GameMenuController : MonoBehaviour
{
    GameMenuController controller = null;
    void Start()
    {
        controller = FindObjectOfType<GameMenuController>();
        controller.CancelFunc = () =>{
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.Disconnect();
        };
    }
}