using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

[RequireComponent(typeof(PC))]
public class N_PC : MonoBehaviour
{
    public Player controller = null;

    void Start()
    {
        if (!GetComponent<PhotonView>().IsMine)
            GetComponent<PC>().enabled = false;
    }

    [PunRPC]
    private void Initialize()
    {
        TeamsManager.AddCharacter(GetComponent<DodgeballCharacter>());
        gameObject.SetActive(false);
    }
}