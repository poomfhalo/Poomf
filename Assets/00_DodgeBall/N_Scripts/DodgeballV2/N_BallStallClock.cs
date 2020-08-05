using UnityEngine;
using Photon.Pun;

public class N_BallStallClock : MonoBehaviour
{
    BallStallClock stallClock = null;
    PhotonView pv = null;

    void Start()
    {
        stallClock = GetComponent<BallStallClock>();
        stallClock.ExtAllowTeleport = () => pv.IsMine;
    }
}