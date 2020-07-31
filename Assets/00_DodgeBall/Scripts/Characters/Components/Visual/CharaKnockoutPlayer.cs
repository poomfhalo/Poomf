using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GW_Lib;
using GW_Lib.Utility;
using System;

public class CharaKnockoutPlayer : MonoBehaviour
{
    public event Action<DodgeballCharacter> E_OnTeleportedOut = null;
    public event Action<DodgeballCharacter> E_OnKnockedOut = null;

    public bool IsInField { get => isInField; private set => isInField = value; }

    [Header("Push Back Data")]
    [SerializeField] float pushForce = 5;
    [Tooltip("Once a Hit is regiestered, character will be pushed back, the push explosion force, will be generated, this distance away from the character")]
    [SerializeField] float pushDist = 1;
    [Tooltip("Zero means, exactly at character feet heigth, -1 means 1 unit above the character feet.")]
    [SerializeField] float yOffset = -0.5f;
    [SerializeField] float explosionRad = 2;
    [SerializeField] ForceMode forceMode = ForceMode.Impulse;
    [SerializeField] bool flipDirection = false;

    [Header("Teleportation Data")]
    [Tooltip("Time before character vanishes")]
    [SerializeField] float teleportStartDelay = 5;
    [Tooltip("time Before Character ReAppears")]
    [SerializeField] float teleportEndDelay = 1.2f;
    [SerializeField] float timeBeforePatrol = 0.5f;

    [Header("Constants")]
    [SerializeField] List<Transform> vTeleportEffectsHead = null;
    [SerializeField] List<Collider> gameCols = new List<Collider>();
    [SerializeField] Transform ragDollHead = null;
    

    [Header("OutOfField Data")]
    [SerializeField] MinMaxRange stopTimeAtPoint = new MinMaxRange(0.2f, 3, 0.5f, 2);
    [SerializeField] bool patrolOutSide = false;
    [Header("ReadOnly")]
    [SerializeField] bool isInField = true;

    List<Collider> ragDollCols = new List<Collider>();
    Vector3 ragDollHeadStartPos = Vector3.zero;
    Quaternion ragDollHeadStartRot = Quaternion.identity;
    CharaHitPoints hp => GetComponent<CharaHitPoints>();
    Animator animator => GetComponent<Animator>();
    CharaSlot slot => GetComponent<CharaSlot>();
    DodgeballCharacter chara => GetComponent<DodgeballCharacter>();
    CharaController charaController => GetComponent<CharaController>();
    Mover mover => GetComponent<Mover>();

    void Awake()
    {
        ragDollCols = ragDollHead.GetComponentsInChildren<Collider>().ToList();
        ragDollCols.RemoveAll(c => gameCols.Contains(c));
        ragDollHeadStartPos = ragDollHead.localPosition;
        ragDollHeadStartRot = ragDollHead.localRotation;

        hp.OnZeroHP += OnZeroHP;
        DisableRagDoll();
        IsInField = true;
    }
    void OnDestroy()
    {
        hp.OnZeroHP -= OnZeroHP;
    }

    public void DisableRagDoll()
    {
        gameCols.ForEach(c => c.enabled = true);
        SetRagPartsState(false);
        ragDollHead.localPosition = ragDollHeadStartPos;
        ragDollHead.localRotation = ragDollHeadStartRot;
        animator.enabled = true;
    }
    public void EnableRagdol()
    {
        gameCols.ForEach(c => c.enabled = false);
        SetRagPartsState(true);
        animator.enabled = false;
    }
    private void SetRagPartsState(bool toState)
    {
        ragDollCols.ForEach(c =>{
            c.enabled = toState;
            Rigidbody r = c.GetComponent<Rigidbody>();
            if (r)
                r.SetKinematic(this, !toState);
        });
    }
    private void OnZeroHP()
    {
        EnableRagdol();
        charaController.Lock();
        IsInField = false;
        E_OnKnockedOut?.Invoke(chara);

        Dodgeball ball = DodgeballGameManager.GetClosestBall(transform);
        Vector3 myFlatPos = transform.position;
        Vector3 ballFlatPos = ball.position;
        myFlatPos.y = ballFlatPos.y = 0;
        Vector3 forceDir = (myFlatPos - ballFlatPos).normalized;
        if (flipDirection)
            forceDir = forceDir * -1;
        Vector3 explosionPos = myFlatPos + forceDir * pushDist;
        ragDollCols.ForEach(rc => {
            Rigidbody rb = rc.GetComponent<Rigidbody>();
            rb.AddExplosionForce(pushForce, explosionPos, explosionRad, yOffset,forceMode);
        });

        if (TeamsManager.GetTeam(chara).IsEmpty)
            return;

        this.InvokeDelayed(teleportStartDelay, () => {
            vTeleportEffectsHead.ForEach(v => GameExtentions.PlayChildEffect(v));
            ragDollHead.gameObject.SetActive(false);

            this.InvokeDelayed(teleportEndDelay, () =>{
                E_OnTeleportedOut?.Invoke(chara);
                GoToWaitField();
            });
        });
    }

    private void GoToWaitField()
    {
        DisableRagDoll();
        CharaPath path = GameExtentions.GetPath(slot.GetID, PathType.OutPath, -1);
        transform.position = path.position;
        transform.rotation = path.rotation;
        ragDollHead.gameObject.SetActive(true);

        this.InvokeDelayed(0.1f, () => {
            vTeleportEffectsHead.ForEach(v => GameExtentions.PlayChildEffect(v));
        });

        this.InvokeDelayed(timeBeforePatrol, () => {
            if(patrolOutSide)
                chara.C_PathFollow(path, true, stopTimeAtPoint.GetValue());

            mover.allowSpeedMultiplication = true;
            GetComponent<Energy>().isInfinity = true;
            charaController.Unlock();
        });
    }
}