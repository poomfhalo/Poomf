using UnityEngine;
using Photon.Pun;

public class N_Energy : MonoBehaviour,IPunObservable
{
    Energy energy = null;
    PhotonView pv = null;

    void Start()
    {
        energy = GetComponent<Energy>();
        pv = GetComponent<PhotonView>();
        energy.ExtAllowWork = () => pv.IsMine;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(energy.GetEnergy());
        }
        else if(stream.IsReading)
        {
            int e = (int)stream.ReceiveNext();
            energy.SetEnergy(e);
        }
    }
}