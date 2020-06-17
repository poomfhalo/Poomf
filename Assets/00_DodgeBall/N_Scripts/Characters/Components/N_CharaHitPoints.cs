using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharaHitPoints))]
public class N_CharaHitPoints : MonoBehaviour
{
    CharaHitPoints hp = null;
    PhotonView pv = null;

    void Awake()
    {
        hp = GetComponent<CharaHitPoints>();
        pv = GetComponent<PhotonView>();
    }
    void Start()
    {
        hp.ApplyHealthChanges = () => false;

        if (PhotonNetwork.IsMasterClient)
            hp.OnHpCommand += OnHpCommand;
    }
    private void OnHpCommand(HPCommand command)
    {
        Log.Message("Sending Health Command " + command, gameObject);
        switch (command)
        {
            case HPCommand.Subtract:
                pv.RPC("R_StartHitAction", RpcTarget.AllViaServer);
                break;
        }
    }

    [PunRPC]
    private void R_StartHitAction()
    {
        Log.Message("N_CharaHitPoints().RPC :: R_StartHitAction");
        hp.ApplyHealthChanges = () => true;
        hp.StartHitAction();
        hp.ApplyHealthChanges = () => false;
    }
}