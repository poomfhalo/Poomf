using UnityEngine;
using Photon.Pun;

public class N_DodgeballThrowSetter : MonoBehaviour
{
    DodgeballThrowSetter setter = null;
    PhotonView pv = null;
    void Start()
    {
        setter = GetComponent<DodgeballThrowSetter>();
        pv = GetComponent<PhotonView>();

        setter.E_OnThrowSelected += OnThrowSelected;
    }

    private void OnThrowSelected()
    {
        if (pv.IsMine)
        {
            pv.RPC("R_LastSetThrowData", RpcTarget.Others, setter.GetLastSelectedThrowData().id);
        }
    }
    [PunRPC]
    private void R_LastSetThrowData(byte id)
    {
        setter.SetThrowDataTo(id);
    }
}
