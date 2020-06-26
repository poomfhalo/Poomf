using System;
using UnityEngine;

[RequireComponent(typeof(DodgeballCharacter))]
public class BallLauncher : DodgeballCharaAction,ICharaAction
{
    public event Action onThrowPointReached = null;
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
    SelectionIndicator selectionIndicator => GetComponent<DodgeballCharacter>().selectionIndicator;

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

    public void A_OnFakeThrowEnded() => isThrowing = false;
    public void A_OnThrowPointReached()
    {
        Dodgeball.instance.transform.SetParent(null);
        animator.SetBool("HasBall", false);
        isThrowing = false;

        Vector3 targetPos = new Vector3();
        if (TeamsManager.AreFriendlies(aimedAtChara, chara))
        {
            //Call, recieving ball or sth here?
            targetPos = aimedAtChara.RecievablePoint.position;
            Debug.LogWarning("Passing To Friendlies has not been implemented yet");
        }
        else
        {
            targetPos = aimedAtChara.ShootablePoint.position;
            Vector3 dir = (targetPos - transform.position).normalized;
            targetPos = targetPos + dir * throwData.ofShootDist;
            Dodgeball.instance.launchTo.C_GoLaunchTo(targetPos, throwData);
            DodgeballGameManager.instance.OnBallThrownAtEnemy(GetComponent<DodgeballCharacter>());
        }

        selectionIndicator.SetFocus(null);
        onThrowPointReached?.Invoke();
    }
    public void A_OnThrowEnded()
    {
        mover.ReadFacingValues();
        if (recievedInput == Vector3.zero)
            return;
        mover.ApplyInput(recievedInput);
    }
}