using System;
using GW_Lib.Utility;
using UnityEngine;

public class CharaHitPoints : DodgeballCharaAction,ICharaAction
{
    public int CurrHP => currHP;
    public event Action OnZeroHP;
    public event Action OnHPUpdated;
    public string actionName => "Get Hit";
    public bool IsBeingHurt => isBeingHurt;
    public bool IsWaitingForHit => isWaitingForHit;

    [SerializeField] int maxHP = 2;
    [SerializeField] int hurtAnimsCount = 2;
    [SerializeField] TriggerDelegator hurtZone = null;

    [Header("Read Only")]
    [SerializeField] int currHP = 0;
    [SerializeField] bool isBeingHurt = false;
    [SerializeField] bool isWaitingForHit = false;

    Animator animator = null;
    ActionsScheduler scheduler = null;

    void Start()
    {
        animator = GetComponent<Animator>();
        scheduler = GetComponent<ActionsScheduler>();

        currHP = maxHP;
        hurtZone.onTriggerEnter.AddListener(OnObjEntered);
    }

    private void OnObjEntered(Collider other)
    {
        Dodgeball ball = other.GetComponent<Dodgeball>();

        if (!ball)
            return;
        if (!IsWaitingForHit)
            return;
        if (isBeingHurt)
            return;

        StartHitAction();
    }

    public void EnableHitDetection()
    {
        isWaitingForHit = true;
    }

    private void StartHitAction()
    {
        currHP = currHP - 1;
        OnHPUpdated?.Invoke();
        if (currHP <= 0)
        {
            OnZeroHP?.Invoke();
            gameObject.SetActive(false);
        }
        else
        {
            isBeingHurt = true;
            int i = UnityEngine.Random.Range(0, hurtAnimsCount);
            animator.SetInteger("RndAnim", i);
            animator.SetTrigger("Hurt");
            scheduler.StartAction(this);
        }
    }
    public void Cancel()
    {

    }

    public void A_OnHitEnded()
    {
        isBeingHurt = false;
        isWaitingForHit = false;
    }
}