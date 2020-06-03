using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;
using System.Linq;

[RequireComponent(typeof(PC))]
public class N_PC : MonoBehaviour,IPunObservable
{
    public int CreatorViewID => creatorViewID;
    public int ActorID => GetComponent<PhotonView>().Controller.ActorNumber;

    [SerializeField] int creatorViewID = 0;

    protected virtual void Start()
    {
        if (!GetComponent<PhotonView>().IsMine)
            GetComponent<PC>().enabled = false;
    }

    [PunRPC]
    private void OnCreated(int creatorViewID)
    {
        this.creatorViewID = creatorViewID;
        TeamsManager.AddCharacter(GetComponent<DodgeballCharacter>());
        gameObject.SetActive(false);
        name = GetComponent<PhotonView>().Controller.NickName;
    }
    [PunRPC]
    private void OnBeforeGameStart()
    {
        SpawnPoint s = FindObjectsOfType<SpawnPoint>().ToList().Find(p => p.CheckPlayer(GetComponent<PhotonView>().Controller.ActorNumber));
        transform.position = s.position;
        transform.rotation = s.rotation;
        gameObject.SetActive(true);

        GetComponent<PC>().SetTeam(N_TeamsManager.GetTeam(ActorID));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}