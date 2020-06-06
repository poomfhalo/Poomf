using System.Collections;
using Photon.Pun;
using UnityEngine;

public class N_PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject pc = null;

    public override void OnEnable()
    {
        base.OnEnable();
        N_GameManager.OnTeamsAreSynced += OnTeamsAreSynced;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        N_GameManager.OnTeamsAreSynced -= OnTeamsAreSynced;
    }

    IEnumerator Start()
    {
        if (photonView.IsMine)
        {
            yield return StartCoroutine(SpawnPC());
        }
        name = "Manager " + GetComponent<PhotonView>().Controller.NickName;
    }
    IEnumerator SpawnPC()
    {
        yield return 0;
        pc = N_Extentions.N_MakeObj(N_Prefab.Player, Vector3.zero,Quaternion.identity);
        pc.GetComponent<PhotonView>().RPC("OnCreated", RpcTarget.All, GetComponent<PhotonView>().ViewID);
        yield return new WaitForSeconds(0.1f);
        Debug.Log(photonView.Controller + " Created a PC ", pc);
        N_Extentions.N_RaiseEvent(N_GameManager.N_OnCreatedPC, null, false);
    }

    private void OnTeamsAreSynced()
    {
        if (pc == null)
            pc = N_Extentions.GetCharacter(GetComponent<PhotonView>().Controller.ActorNumber).gameObject;
    }
}
