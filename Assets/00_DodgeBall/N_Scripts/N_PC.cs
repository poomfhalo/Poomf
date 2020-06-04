using UnityEngine;
using Photon.Pun;
using System.Linq;

public class N_PC : MonoBehaviour,IPunObservable
{
    public int CreatorViewID => creatorViewID;
    public int ActorID => GetComponent<PhotonView>().Controller.ActorNumber;

    [SerializeField] int creatorViewID = 0;
    protected PC pc = null;
    DodgeballCharacter chara = null;
    protected virtual void Start()
    {
        pc = GetComponent<PC>();
        if (!GetComponent<PhotonView>().IsMine)
        {
            pc.enabled = false;
        }
    }
    void OnEnable()
    {
        chara = GetComponent<DodgeballCharacter>();
        if(GetComponent<PhotonView>().IsMine)
            chara.OnCommandActivated += SendCommand;
    }
    void OnDisable()
    {
        if (GetComponent<PhotonView>().IsMine)
            chara.OnCommandActivated -= SendCommand;
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
    private void PrepareForGame()
    {
        SpawnPoint s = FindObjectsOfType<SpawnPoint>().ToList().Find(p => p.CheckPlayer(GetComponent<PhotonView>().Controller.ActorNumber));
        transform.position = s.position;
        transform.rotation = s.rotation;
        gameObject.SetActive(true);

        GetComponent<DodgeballCharacter>().SetTeam(N_TeamsManager.GetTeam(ActorID));
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(chara.syncedInput.x);
            stream.SendNext(chara.syncedInput.z);
            stream.SendNext(chara.syncedYAngle);
        }
        else if (stream.IsReading)
        {
            chara.syncedInput.x = (float)stream.ReceiveNext();
            chara.syncedInput.z = (float)stream.ReceiveNext();
            chara.syncedYAngle = (float)stream.ReceiveNext();
        }
    }

    private void SendCommand(DodgeballCharaCommand command)
    {
        GetComponent<PhotonView>().RPC("RecieveCommand", RpcTarget.Others, (int)command);
    }

    [PunRPC]
    private void RecieveCommand(int c)
    {
        DodgeballCharaCommand command = (DodgeballCharaCommand)c;
        switch (command)
        {
            case DodgeballCharaCommand.Catch:
                chara.C_Catch();
                break;
            case DodgeballCharaCommand.Dodge:
                chara.C_Dodge();
                break;
            case DodgeballCharaCommand.Enemy:
                chara.C_Enemy();
                break;
            case DodgeballCharaCommand.FakeFire:
                chara.C_FakeFire();
                break;
            case DodgeballCharaCommand.Fire:
                chara.C_Fire();
                break;
            case DodgeballCharaCommand.Friendly:
                chara.C_Friendly();
                break;
            case DodgeballCharaCommand.Jump:
                chara.C_Jump();
                break;
            case DodgeballCharaCommand.MoveInput:
                chara.C_MoveInput();
                break;
        }
    }
}