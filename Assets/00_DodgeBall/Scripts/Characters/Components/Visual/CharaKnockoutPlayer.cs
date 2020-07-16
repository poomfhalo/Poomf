using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GW_Lib;
using GW_Lib.Utility;
using System;

public class CharaKnockoutPlayer : MonoBehaviour
{
    public event Action<DodgeballCharacter> E_OnKnockedOut = null;
    public bool IsInField { get => isInField; private set => isInField = value; }

    [SerializeField] List<Transform> vTeleportEffectsHead = null;
    [SerializeField] List<Collider> gameCols = new List<Collider>();
    [SerializeField] Transform ragDollHead = null;
    [Header("OutOfField Data")]
    [SerializeField] MinMaxRange stopTimeAtPoint = new MinMaxRange(0.2f, 3, 0.5f, 2);

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
        this.InvokeDelayed(3f, () => {
            vTeleportEffectsHead.ForEach(v => GameExtentions.PlayChildEffect(v));
            DisableRagDoll();
            ragDollHead.gameObject.SetActive(false);

            this.InvokeDelayed(1.2f, () =>{
                TeamsManager.GetEmptyTeams(out bool a, out bool b);
                E_OnKnockedOut?.Invoke(chara);
                GoToWaitField();
            });
        });
    }

    private void GoToWaitField()
    {
        CharaPath path = GameExtentions.GetPath(slot.GetID, PathType.OutPath, -1);
        transform.position = path.position;
        transform.rotation = path.rotation;
        ragDollHead.gameObject.SetActive(true);
        vTeleportEffectsHead.ForEach(v => GameExtentions.PlayChildEffect(v));

        this.InvokeDelayed(2, () => {
            chara.C_PathFollow(path, true, stopTimeAtPoint.GetValue());
        });
    }
}