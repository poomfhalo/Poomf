using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;
using System.Linq;

[RequireComponent(typeof(PC))]
public class N_PC : MonoBehaviour
{
    public int Maker => maker;
    [SerializeField] int maker = 0;

    void Start()
    {
        if (!GetComponent<PhotonView>().IsMine)
            GetComponent<PC>().enabled = false;
    }

    [PunRPC]
    private void OnCreated(int maker)
    {
        this.maker = maker;
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
    }
}