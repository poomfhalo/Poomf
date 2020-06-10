using System;
using GW_Lib;
using UnityEngine;

[RequireComponent(typeof(DodgeballCharacter))]
public class BallLauncher : DodgeballCharaAction,ICharaAction
{
    public event Action onThrowPointReached = null;
    public event Action onFakeThrowEnded = null;

    public bool IsThrowing => isThrowing;
    public string actionName => activityName;
    [Tooltip("If the value is less than 1 then we will use the Mover turn speed, noting that, if its too low, character may not" +
    "\nface the target, by the end of the animation, but the ball, will still travel towards the target")]
    [SerializeField] float throwFacingSpeed = 200;

    [SerializeField] float heigth = 10;
    [SerializeField] float gravity = 5;
    [SerializeField] BallThrowData throwData = null;

    [Header("Read Only")]
    [SerializeField] bool isThrowing = false;
    [SerializeField] string activityName = "Throw Action";

    DodgeballCharacter chara = null;
    Animator animator = null;
    ActionsScheduler scheduler = null;
    DodgeballCharacter aimedAtChara = null;
    Mover mover = null;

    void Awake()
    {
        chara = GetComponent<DodgeballCharacter>();
        animator = GetComponent<Animator>(); 
        scheduler = GetComponent<ActionsScheduler>();
        mover = GetComponent<Mover>();
    }
    void Update()
    {
        if (IsThrowing)
        {
            mover.TurnToPoint(aimedAtChara.position, throwFacingSpeed);
        }
    }

    public void Cancel()
    {

    }
    public void StartThrowAction(DodgeballCharacter toChara)
    {
        if (isThrowing)
            return;

        isThrowing = true;
        aimedAtChara = toChara;
        Action a = () =>{
            animator.SetTrigger("Throw");
            activityName = "Throw Action";
            scheduler.StartAction(this,false);
        };
        if (mover.IsMoving)
            mover.SmoothStop(a);
        else
            a.Invoke();
    }
    public void StartFakeThrow(DodgeballCharacter activeChara)
    {
        if (isThrowing)
            return;

        isThrowing = true;
        aimedAtChara = activeChara;

        Action a = () => {
            animator.SetTrigger("FakeThrow");
            activityName = "Fake Throw Action";
            scheduler.StartAction(this,false);
        };
        if (mover.IsMoving)
            mover.SmoothStop(a);
        else
            a.Invoke();
    }

    public void A_OnFakeThrowEnded()
    {
        isThrowing = false;
        onFakeThrowEnded?.Invoke();
    }
    public void A_OnThrowPointReached()
    {
        Dodgeball.instance.transform.SetParent(null);
        if (TeamsManager.AreFriendlies(aimedAtChara, chara))
        {
            aimedAtChara.C_EnableReciption();
            Debug.LogWarning("Passing To Friendlies has not been implemented yet");
        }
        else
        {
            animator.SetBool("HasBall", false);
            isThrowing = false;
            aimedAtChara.C_EnableHit();
            Dodgeball.instance.launchTo.C_GoLaunchTo(aimedAtChara.ShootablePoint.position, throwData);
            onThrowPointReached?.Invoke();
        }
    }
    public void A_OnThrowEnded()
    {
        mover.ReadFacingValues();
        if (recievedInput == Vector3.zero)
            return;
        mover.ApplyInput(recievedInput);
    }
}