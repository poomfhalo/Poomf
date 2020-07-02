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
    public bool IsInHitCd => isInHitCd;

    [SerializeField] int maxHP = 2;
    [SerializeField] int hurtAnimsCount = 2;
    [SerializeField] TriggerDelegator hurtZone = null;
    [SerializeField] float minTimeBetweenHurts = 1;
    public Func<bool> ApplyHealthChanges = () => true;

    [Header("Read Only")]
    [SerializeField] int currHP = 0;
    [SerializeField] bool isBeingHurt = false;
    [SerializeField] bool isWaitingForHit = false;
    [SerializeField] bool isInHitCd = false;

    Animator animator = null;
    ActionsScheduler scheduler = null;
    Rigidbody rb3d = null;
    Mover mover = null;

    void Start()
    {
        animator = GetComponent<Animator>();
        scheduler = GetComponent<ActionsScheduler>();
        rb3d = GetComponent<Rigidbody>();
        mover = GetComponent<Mover>();

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
        if (IsInHitCd)
            return;

        C_StartHitAction();
    }

    public void EnableHitDetection()
    {
        isWaitingForHit = true;
    }
    public void C_StartHitAction()
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
            if(animator.runtimeAnimatorController == null)
            {
                this.InvokeDelayed(0.1f, A_OnHitEnded);
            }
        }
    }
    public void Cancel()
    {

    }
    public void A_OnHitEnded()
    {
        //TODO: Implement check, for when, we "disable" characters, in game, instead of turning off game obj
        if (!gameObject.activeSelf)
            return;

        isInHitCd = true;
        this.InvokeDelayed(minTimeBetweenHurts, () =>{
            isInHitCd = false;
        });

        isBeingHurt = false;
        isWaitingForHit = false;

        mover.ReadFacingValues();
        if (recievedInput == Vector3.zero)
            return;
        mover.ApplyInput(recievedInput);
    }

    public void DisableHitDetection()
    {
        A_OnHitEnded();
    }
}