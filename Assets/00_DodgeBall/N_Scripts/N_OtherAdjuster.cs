using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(N_PC))]
public class N_OtherAdjuster : MonoBehaviour
{
    [SerializeField] float ballGrabZoneSkin = 0.1f;
    [SerializeField] BoxCollider ballGrabZone = null;
    PhotonView pv = null;
    N_PC n_pc = null;

    void OnEnable()
    {
        pv = GetComponent<PhotonView>();
        n_pc = GetComponent<N_PC>();

        if(!pv.IsMine)
        {
            ApplyAdjustments();
        }
    }

    private void ApplyAdjustments()
    {
        float increment = n_pc.autoMoveThreshold + ballGrabZoneSkin;
        ballGrabZone.size = ballGrabZone.size + increment * Vector3.up;
    }
}