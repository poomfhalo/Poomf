using UnityEngine;
using Photon.Pun;

public class N_MatchResult : MonoBehaviour
{
    MatchResult mr = null;
    void Awake()
    {
        mr = GetComponent<MatchResult>();
        mr.beforeChangingScene += () =>{
            if(PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LeaveLobby();
                PhotonNetwork.Disconnect();
                PhotonNetwork.AutomaticallySyncScene = false;
            }
        };
    }
}