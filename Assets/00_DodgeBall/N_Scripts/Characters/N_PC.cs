﻿using GW_Lib;
using Photon.Pun;
using UnityEngine;

public class N_PC : MonoBehaviour,IPunObservable
{
    public int CreatorViewID => creatorViewID;
    public int ActorID => pv.Controller.ActorNumber;
    [SerializeField] int creatorViewID = 0;
    [SerializeField] float afterDodgeMovementBlock = 0.1f;
    [SerializeField] float safeThrowPercentThreshold = 0.15f;


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

    void Awake()
    {
        GetComponent<CharaSlot>().setActiveOnStart = false;
    }
    void Start()
    {
        BallLauncherV2 lV2 = chara.launcher as BallLauncherV2;
        if (lV2 && PhotonNetwork.IsMasterClient)
        {
            lV2.travelPercentToThrow = safeThrowPercentThreshold;
        }
    }
    void OnEnable()
    {
        pv = GetComponent<PhotonView>();
        pc = GetComponent<PC>();

        pc.extAllowInputOnStart = pv.IsMine;
        pc.enabled = pv.IsMine;

        rb3d = GetComponent<Rigidbody>();
        chara = GetComponent<DodgeballCharacter>();
        if (pv.IsMine)
        {
            chara.OnCommandActivated += SendMyCommand;
            chara.GetComponent<Mover>().movementType = Mover.MovementType.ByInput;
            GetComponent<CharaController>().moveTypeOnUnlock = Mover.MovementType.ByInput;
        }
        else
        {
            chara.GetComponent<Mover>().movementType = Mover.MovementType.ToPoint;
            GetComponent<CharaController>().moveTypeOnUnlock = Mover.MovementType.ToPoint;
            chara.GetComponentInChildren<CharaFeet>().extCanPush = false;
            GetComponent<PathFollower>().extCanPlayAction = false;
        }

        chara.launcher.ExtThrowCondition = PhotonNetwork.IsMasterClient;
        chara.launcher.E_OnBallLaunchedSafely += OnBallLaunchedSafely;
        chara.launcher.E_OnThrowPointReached += OnThrowPointReached;
    }
    void OnDisable()
    {
        if (pv.IsMine)
            chara.OnCommandActivated -= SendMyCommand;

        chara.launcher.E_OnThrowP1Finished -= OnBallLaunchedSafely;
        chara.launcher.E_OnThrowPointReached -= OnThrowPointReached;
    }

    [PunRPC]//Called In N_PlayerManager
    private void OnCreated(int creatorViewID)
    {
        this.creatorViewID = creatorViewID;
        gameObject.SetActive(false);
        name = pv.Controller.NickName;
    }
    [PunRPC]//Called In N_GameManager
    private void PrepareForGame()
    {
        netPos = GetComponent<DodgeballCharacter>().PrepareForGame();
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

    private void SendMyCommand(DodgeballCharaCommand command)//We only reach this step if IsMine is true
    {
        float currX = rb3d.position.x;
        float currZ = rb3d.position.z;

        if(command == DodgeballCharaCommand.Dodge)
        {
            Dodger d = GetComponent<Dodger>();

            Vector3 startPos = transform.position;
            Vector3 expectedPos = d.GetExpectedPosition();

            pv.RPC("R_DodgeCommand", RpcTarget.Others, startPos.x, startPos.z, expectedPos.x, expectedPos.z);
        }
        else if (command == DodgeballCharaCommand.PushBall)
        {
            CharaFeet feet = GetComponentInChildren<CharaFeet>();
            //pv.RPC("R_FeetPush", RpcTarget.Others, feet.lastPushUsed,currX,currZ);
        }
        else if (command == DodgeballCharaCommand.PathFollow)
        {
            PathFollower follower = GetComponent<PathFollower>();
            CharaPath p = follower.ActivePath;
            pv.RPC("R_PathFollow", RpcTarget.Others, currX, currZ, (int)p.pathType, p.name,follower.isLooping,follower.stopTimeAtPoint);
        }
        else
        {
            pv.RPC("R_Command", RpcTarget.Others, (int)command, currX, currZ);
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
    private void R_Command(int c, float currX, float currZ)
    {
        netPos.x = currX;
        netPos.z = currZ;
        UpdateNetData();

        DodgeballCharaCommand command = (DodgeballCharaCommand)c;
        if (command == DodgeballCharaCommand.BraceForBall || command == DodgeballCharaCommand.ReleaseFromBrace)
        {
            Log.Message("N_PC().RPC :: R_Command " + command);
        }
        else
        {
            Log.LogL0("N_PC().RPC :: R_Command " + command);
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
    private void R_DodgeCommand(float startX,float startZ,float expectedX,float expectedZ)
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
    [PunRPC]
    private void R_FeetPush(Vector3 lastUsedForce,float x, float z)
    {
        lastCommand = DodgeballCharaCommand.PushBall;
        netPos.x = x;
        netPos.z = z;
        if (Dodgeball.instance.ballState != Dodgeball.BallState.OnGround)
            return;

        CharaFeet feet = GetComponentInChildren<CharaFeet>();
        feet.ApplyPush(Dodgeball.instance, lastUsedForce);
        Log.Warning("Pushed Ball By Feet");
    }
    [PunRPC]
    private void R_PathFollow(float currX,float currZ,int pathTag,string pathName,bool isLooping,float stopTimeAtPoint)
    {
        lastCommand = DodgeballCharaCommand.PathFollow;
        netPos.x = currX;
        netPos.z = currZ;

        int slotID = GetComponent<CharaSlot>().GetID;
        CharaPath path = GameExtentions.GetPath(slotID, (PathType)pathTag, pathName);
        chara.C_PathFollow(path,isLooping,stopTimeAtPoint);
    }
    //Local Events
    private void OnBallLaunchedSafely()
    {
        Log.Message("N_PC().OnBallLaunchedSafely :: " + name + " :: Is Master " + PhotonNetwork.IsMasterClient, gameObject);
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("R_ActivateBallThrow", RpcTarget.Others);
        }
    }
    [PunRPC]
    private void R_ActivateBallThrow()
    {
        Log.Message("N_PC().R_ActivateBallThrow :: " + name, gameObject);
        chara.launcher.ExtThrowCondition = true;
    }
    private void OnThrowPointReached()
    {
        Log.Message("N_PC()."+ name + " :: Resetting External Throw Condition");
        chara.launcher.ExtThrowCondition = PhotonNetwork.IsMasterClient;
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
        netDist = Vector3.Distance(currPos, netPos);

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
        if (lastCommand == DodgeballCharaCommand.PathFollow)
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