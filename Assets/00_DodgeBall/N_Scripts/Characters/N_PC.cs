using UnityEngine;
using Photon.Pun;
using System.Linq;
using GW_Lib;

public class N_PC : MonoBehaviour,IPunObservable
{
    public int CreatorViewID => creatorViewID;
    public int ActorID => pv.Controller.ActorNumber;
    [SerializeField] int creatorViewID = 0;
    [SerializeField] float afterDodgeMovementBlock = 0.1f;

    [Header("Move Smoothing Settings")]
    [Tooltip("if this distance between current position and networked position is higher than this, we snap to correct XZ place")]
    [SerializeField] float snapXZDist = 2;
    [Tooltip("Distance (Networked Pos/Current Position) of which above we will keep trying to actively move")]
    public float autoMoveThreshold = 0.3f;

    PC pc = null;
    DodgeballCharacter chara = null;
    Rigidbody rb3d = null;
    PhotonView pv = null;

    [Header("Read Only")]
    [SerializeField] float netDist = 0;
    [SerializeField] Vector3 netPos = new Vector3();
    [SerializeField] Vector3 netDisp = new Vector3();
    [SerializeField] Vector3 netDir = new Vector3();
    [SerializeField] DodgeballCharaCommand lastCommand = DodgeballCharaCommand.MoveInput;
    bool firstRead = true;
    bool canCallMovement = true;

    void OnEnable()
    {
        pv = GetComponent<PhotonView>();
        pc = GetComponent<PC>();
        if (!pv.IsMine)
        {
            pc.enabled = false;
        }
        rb3d = GetComponent<Rigidbody>();
        chara = GetComponent<DodgeballCharacter>();
        if (GetComponent<PhotonView>().IsMine)
        {
            chara.OnCommandActivated += SendCommand;
            chara.GetComponent<Mover>().movementMode = Mover.MovementType.ByInput;
        }
        else
        {
            chara.GetComponent<Mover>().movementMode = Mover.MovementType.ToPoint;
        }
    }
    void OnDisable()
    {
        if (pv.IsMine)
            chara.OnCommandActivated -= SendCommand;
    }

    [PunRPC]//Called In N_PlayerManager
    private void OnCreated(int creatorViewID)
    {
        this.creatorViewID = creatorViewID;
        TeamsManager.AddCharacter(GetComponent<DodgeballCharacter>());
        gameObject.SetActive(false);
        name = pv.Controller.NickName;
    }
    [PunRPC]//Called In N_GameManager
    private void PrepareForGame()
    {
        SpawnPoint s = FindObjectsOfType<SpawnPoint>().ToList().Find(p => p.CheckPlayer(pv.Controller.ActorNumber));
        transform.position = s.position;
        transform.rotation = s.rotation;
        netPos = s.position;

        gameObject.SetActive(true);

        GetComponent<DodgeballCharacter>().SetTeam(N_TeamsManager.GetTeam(ActorID));
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (lastCommand == DodgeballCharaCommand.Dodge)
                return;

            stream.SendNext(rb3d.position.x);
            stream.SendNext(rb3d.position.z);
        }
        else if (stream.IsReading)
        {
            if (firstRead)
            {
                firstRead = false;
                stream.ReceiveNext();
                stream.ReceiveNext();
                return;
            }
            if (lastCommand == DodgeballCharaCommand.Dodge)
                return;

            netPos.x = (float)stream.ReceiveNext();
            netPos.z = (float)stream.ReceiveNext();
            UpdateNetData();

            TrySnapToNetPos();
            UpdateSyncedInput();
        }
    }

    void FixedUpdate()
    {
        UpdateNetData();
        UpdateSyncedInput();
    }

    private void SendCommand(DodgeballCharaCommand command)
    {
        float currX = rb3d.position.x;
        float currZ = rb3d.position.z;

        if(command == DodgeballCharaCommand.Dodge)
        {
            Dodger d = GetComponent<Dodger>();

            Vector3 startPos = transform.position;
            Vector3 expectedPos = d.GetExpectedPosition();

            pv.RPC("RecieveDodgeCommand", RpcTarget.Others, startPos.x, startPos.z, expectedPos.x, expectedPos.z);
        }
        else
        {
            pv.RPC("RecieveCommand", RpcTarget.Others, (int)command, currX, currZ);
        }

        if (command == DodgeballCharaCommand.BraceForBall || command == DodgeballCharaCommand.ReleaseFromBrace)
        {
            Log.Message("N_PC().SendCommand :: " + command);
        }
        else
        {
            Log.LogL0("N_PC().SendCommand :: " + command);
        }
    }

    [PunRPC]
    private void RecieveCommand(int c, float currX, float currZ)
    {
        netPos.x = currX;
        netPos.z = currZ;
        UpdateNetData();

        DodgeballCharaCommand command = (DodgeballCharaCommand)c;
        if (command == DodgeballCharaCommand.BraceForBall || command == DodgeballCharaCommand.ReleaseFromBrace)
        {
            Log.Message("N_PC().RPC :: Recieved Command " + command);
        }
        else
        {
            Log.LogL0("N_PC().RPC :: RecieveCommand " + command);
        }

        switch (command)
        {
            case DodgeballCharaCommand.Enemy:
                chara.C_Enemy();
                break;
            case DodgeballCharaCommand.FakeFire:
                chara.C_FakeFire();
                break;
            case DodgeballCharaCommand.BallAction:
                chara.C_OnBallAction(UnityEngine.InputSystem.InputActionPhase.Started);
                break;
            case DodgeballCharaCommand.Friendly:
                chara.C_Friendly();
                break;
            case DodgeballCharaCommand.Jump:
                chara.C_Jump();
                break;
            case DodgeballCharaCommand.MoveInput:
                if (lastCommand == DodgeballCharaCommand.Dodge)
                {
                    lastCommand = DodgeballCharaCommand.MoveInput;
                    //GetComponent<Mover>().Warp(netPos);
                    transform.position = netPos;
                    //So character does not attempt to move back and forth, at the targeted point
                    chara.C_MoveInput(netPos);
                    Log.Warning("Snapped Up, Dodge Net Position", gameObject);
                    this.InvokeDelayed(afterDodgeMovementBlock, () => canCallMovement = true);
                    return;
                }
                UpdateSyncedInput();
                break;
            case DodgeballCharaCommand.BraceForBall:
                chara.C_BraceForContact();
                break;
        }

        lastCommand = command;
    }   
    [PunRPC]
    private void RecieveDodgeCommand(float startX,float startZ,float expectedX,float expectedZ)
    {
        lastCommand = DodgeballCharaCommand.Dodge;
        canCallMovement = false;

        transform.position = new Vector3(startX, transform.position.y, startZ);
        netPos = new Vector3(expectedX, transform.position.y, expectedZ);

        Vector3 dir = netPos - transform.position;
        dir.y = 0;
        dir.Normalize();
        transform.rotation = Quaternion.LookRotation(dir);

        GetComponent<Dodger>().StartDodgeAction(netPos, null);
    }
    //Helper Functions
    private void UpdateNetData()
    {
        netDisp = (rb3d.position - netPos);
        netDisp.y = 0;
        netDist = netDisp.magnitude;
        netDir = netDisp.normalized;
    }
    private void TrySnapToNetPos()
    {
        Vector3 currPos = rb3d.position;
        currPos.y = 0;
        netDist = Vector3.Distance(rb3d.position, netPos);

        if (netDist > snapXZDist)
        {
            rb3d.MovePosition(netPos);
        }
    }
    private void UpdateSyncedInput()
    {
        if (pv.IsMine)
            return;
        if (netDist < autoMoveThreshold)
            return;
        if (lastCommand == DodgeballCharaCommand.Dodge)
            return;
        if (!canCallMovement)
            return;

        chara.C_MoveInput(netPos);
    }

    void OnDrawGizmos()
    {
        Color c = Color.red;
        c.a = 0.5f;
        Gizmos.color = c;

        Gizmos.DrawSphere(netPos, autoMoveThreshold);
    }
}