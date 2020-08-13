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
        hp.ExtApplyHealthChanges = () => false;

        if (PhotonNetwork.IsMasterClient)
            hp.OnHpCommand += OnHpCommand;
    }
    private void OnHpCommand(HPCommand command)
    {
        Log.Message("Sending Health Command " + command, gameObject);
        switch (command)
        {
            case HPCommand.Subtract:
                int lastHitter = hp.lastDamager.GetComponent<N_PC>().ActorID;
                pv.RPC("R_StartHitAction", RpcTarget.AllViaServer, lastHitter);
                break;
        }
    }

    [PunRPC]
    private void R_StartHitAction(int lastHitter)
    {
        Log.Message("N_CharaHitPoints().RPC :: R_StartHitAction");
        hp.ExtApplyHealthChanges = () => true;
        DodgeballCharacter chara = N_TeamsManager.GetPlayer(lastHitter).GetComponent<DodgeballCharacter>();
        hp.StartHitAction(1,chara);
        hp.ExtApplyHealthChanges = () => false;
    }
}