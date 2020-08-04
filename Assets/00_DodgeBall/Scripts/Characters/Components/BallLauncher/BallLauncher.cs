using System;
using UnityEngine;

public abstract class BallLauncher : DodgeballCharaAction, ICharaAction,IEnergyAction
{
    public event Action E_OnThrowStarted = null;
    public event Action E_OnThrowP1Finished = null;
    public event Action E_OnThrowPointReached = null;
    public event Action E_OnBallLaunchedSafely = null;
    public event Action E_OnThrowAnimEnded = null;

    public bool IsThrowing => isThrowing;
    public string actionName => activityName;

    [SerializeField] int fakeThrowEnergyCost = 5;
    [Header("Throw Data")]
    [Tooltip("If the value is less than 1 then we will use the Mover turn speed, noting that, if its too low, character may not" +
"\nface the target, by the end of the animation, but the ball, will still travel towards the target")]
    [SerializeField] float throwFacingSpeed = 200;
    [SerializeField] BallThrowData throwData = null;

    [Header("Read Only")]
    [SerializeField] protected bool isThrowing = false;
    [SerializeField] protected string activityName = "";
    public bool ExtThrowCondition = true;
    public bool isLastThrowAtEnemy = false;

    protected DodgeballCharacter chara = null;
    protected Animator animator = null;
    protected ActionsScheduler scheduler = null;
    protected Mover mover = null;
    protected Jumper jumper = null;
    protected DodgeballCharacter aimedAtChara = null;
    protected SelectionIndicator selectionIndicator => GetComponent<DodgeballCharacter>().selectionIndicator;

    public Action<int> ConsumeEnergy { get; set; }
    public Func<int, bool> CanConsumeEnergy { get; set; }
    public Func<bool> AllowRegen => () => !IsThrowing;

    protected virtual void Awake()
    {
        chara = GetComponent<DodgeballCharacter>();
        animator = GetComponent<Animator>();
        scheduler = GetComponent<ActionsScheduler>();
        mover = GetComponent<Mover>();
        jumper = GetComponent<Jumper>();
    }
    protected virtual void Update()
    {
        if (IsThrowing && aimedAtChara)
            mover.TurnToPoint(aimedAtChara.position, throwFacingSpeed);
    }

    public abstract void StartThrowAction(DodgeballCharacter toChara);
    public virtual void StartFakeThrow(DodgeballCharacter activeChara)
    {
        if (isThrowing)
            return;
        if (!CanConsumeEnergy(fakeThrowEnergyCost))
            return;

        RunOnBallThrowStart();
        ConsumeEnergy(fakeThrowEnergyCost);
        isThrowing = true;
        aimedAtChara = activeChara;

        Action a = () =>{
            animator.SetTrigger("FakeThrow");
            activityName = "Fake Throw ActionV2";
            scheduler.StartAction(this, false);
        };
        if (mover.IsMoving)
            mover.SmoothStop(a);
        else
            a.Invoke();
    }
    public virtual void A_OnFakeThrowEnded()
    {
        isThrowing = false;
        RunOnThrowAnimEnded();
    }
    public virtual void A_OnThrowPointReached()
    {
        if (!aimedAtChara)
            return;
        Dodgeball.instance.transform.SetParent(null);
        animator.SetBool("HasBall", false);
        isThrowing = false;

        Vector3 targetPos = new Vector3();
        if (!isLastThrowAtEnemy)
            targetPos = aimedAtChara.RecievablePoint.position;
        else
            targetPos = aimedAtChara.ShootablePoint.position;

        Vector3 dir = (targetPos - transform.position).normalized;
        targetPos = targetPos + dir * throwData.ofShootDist;
        Dodgeball.instance.launchTo.C_GoLaunchTo(targetPos, throwData);

        if (!isLastThrowAtEnemy)
        {
            DodgeballGameManager.instance.OnBallThrownAtAlly(GetComponent<DodgeballCharacter>());
        }
        else
        {
            DodgeballGameManager.instance.OnBallThrownAtEnemy(GetComponent<DodgeballCharacter>());
        }

        selectionIndicator.SetFocus(null);
        RunOnThrowPointReached();
        RunOnThrowAnimEnded();
    }
    public virtual void A_OnThrowEnded()
    {
        mover.ReadFacingValues();
        if (recievedInput == Vector3.zero)
            return;
        mover.ApplyInput(recievedInput);
    }

    public virtual void Cancel()
    {

    }
    protected virtual void RunThrowP1Finished()
    {
        E_OnThrowP1Finished?.Invoke();
    }
    protected virtual void RunOnThrowPointReached()
    {
        E_OnThrowPointReached?.Invoke();
    }
    protected virtual void RunOnBallLaunchedSafely()
    {
        E_OnBallLaunchedSafely?.Invoke();
    }
    protected virtual void RunOnBallThrowStart()
    {
        E_OnThrowStarted?.Invoke();
    }
    protected virtual void RunOnThrowAnimEnded() => E_OnThrowAnimEnded?.Invoke();
}