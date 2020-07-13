using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GW_Lib;

public class CharaRagPlayer : MonoBehaviour
{
    [SerializeField] List<Collider> gameCols = new List<Collider>();
    [SerializeField] Transform ragDollHead = null;

    List<Collider> ragDollCols = new List<Collider>();
    Vector3 ragDollHeadStartPos = Vector3.zero;
    CharaHitPoints hp => GetComponent<CharaHitPoints>();
    Animator animator => GetComponent<Animator>();

    void Awake()
    {
        ragDollCols = ragDollHead.GetComponentsInChildren<Collider>().ToList();
        ragDollCols.RemoveAll(c => gameCols.Contains(c));
        ragDollHeadStartPos = ragDollHead.position;

        hp.OnZeroHP += OnZeroHP;
        DisableRagDoll();
    }
    void OnDestroy()
    {
        hp.OnZeroHP -= OnZeroHP;
    }

    public void DisableRagDoll()
    {
        gameCols.ForEach(c => c.enabled = true);
        SetRagPartsState(false);
        ragDollHead.position = ragDollHeadStartPos;
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
        this.InvokeDelayed(3f, () => { gameObject.SetActive(false); });
    }
}