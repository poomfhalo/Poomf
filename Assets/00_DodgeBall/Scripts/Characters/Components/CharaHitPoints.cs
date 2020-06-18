using System;
using GW_Lib;
using GW_Lib.Utility;
using UnityEngine;

public enum HPCommand { Subtract } 
public class CharaHitPoints : DodgeballCharaAction,ICharaAction
{
    public int CurrHP => currHP;
    public event Action OnZeroHP;
    public event Action OnHPUpdated;
    public event Action<HPCommand> OnHpCommand;
    public string actionName => "Get Hit";
    public bool IsBeingHurt => isBeingHurt;
    public bool IsWaitingForHit => isWaitingForHit;

    [SerializeField] int maxHP = 2;
    [SerializeField] int hurtAnimsCount = 2;
    [SerializeField] TriggerDelegator hurtZone = null;
    public Func<bool> ApplyHealthChanges = () => true;

    [Header("Read Only")]
    [SerializeField] int currHP = 0;
    [SerializeField] bool isBeingHurt = false;
    [SerializeField] bool isWaitingForHit = false;

    Animator animator = null;
    ActionsScheduler scheduler = null;
    Rigidbody rb3d = null;

    void Start()
    {
        animator = GetComponent<Animator>();
        scheduler = GetComponent<ActionsScheduler>();
        rb3d = GetComponent<Rigidbody>();

        currHP = maxHP;
        hurtZone.onTriggerEnter.AddListener(OnObjEntered);
        OnHPUpdated?.Invoke();
    }

    private void OnObjEntered(Collider other)
    {
        Dodgeball ball = other.GetComponent<Dodgeball>();

        if(!other.isTrigger)//Since the ball has 2 colliders, we only want to account for the trigger.
            return;
        if (!ball)
            return;
        if (!IsWaitingForHit)
            return;
        if (isBeingHurt)
            return;

        C_StartHitAction(other);
    }

    public void EnableHitDetection()
    {
        isWaitingForHit = true;
    }
    private void C_StartHitAction(Collider other)
    {
        if (!IsWaitingForHit)
            return;

        OnHpCommand?.Invoke(HPCommand.Subtract);
        isWaitingForHit = false;

        if (!ApplyHealthChanges())
            return;

        StartHitAction();
    }

    public void StartHitAction()
    {
        if (!ApplyHealthChanges())
            return;

        currHP = currHP - 1;
        OnHPUpdated?.Invoke();
        this.SetKinematic(true);
        if (currHP <= 0)
        {
            OnZeroHP?.Invoke();
            gameObject.SetActive(false);
            Log.Message("HP: Zero Health", gameObject);
        }
        else
        {
            isBeingHurt = true;
            int i = UnityEngine.Random.Range(0, hurtAnimsCount);
            animator.SetInteger("RndAnim", i);
            animator.SetTrigger("Hurt");
            scheduler.StartAction(this);
            Log.Message("HP: Health Reduced", gameObject);
        }
    }
    public void Cancel()
    {

    }
    public void A_OnHitEnded()
    {
        isBeingHurt = false;
        isWaitingForHit = false;
        this.SetKinematic(false);
    }

    public void DisableHitDetection()
    {
        A_OnHitEnded();
    }
}