using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

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
    private void Initialize(int maker)
    {
        this.maker = maker;
        TeamsManager.AddCharacter(GetComponent<DodgeballCharacter>());
        gameObject.SetActive(false);
        name = PhotonNetwork.NickName;
    }
}