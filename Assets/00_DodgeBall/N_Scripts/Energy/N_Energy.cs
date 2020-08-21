using UnityEngine;
using Photon.Pun;

public class N_Energy : MonoBehaviour,IPunObservable
{
    [SerializeField] Energy energy = null;
    PhotonView pv = null;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        energy.ExtAllowWork = () => pv.IsMine;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            int f = energy.GetEnergy();
            stream.SendNext(f);
        }
        else if (stream.IsReading)
        {
            var o = stream.ReceiveNext();
            int e = (int)o;
            energy.SetEnergy(e);
        }
    }
}