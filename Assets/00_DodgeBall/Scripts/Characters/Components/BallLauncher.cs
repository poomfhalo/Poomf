using System;
using GW_Lib;
using UnityEngine;

[RequireComponent(typeof(DodgeballCharacter))]
public class BallLauncher : MonoBehaviour,ICharaAction
{
    public event Action onThrowPointReached = null;
    public event Action onFakeThrowEnded = null;

    public bool IsThrowing => isThrowing;
    public string actionName => activityName;

    [SerializeField] float throwFacingSpeed = 200;
    [Header("Throw Data")]
    [SerializeField] float heigth = 10;
    [SerializeField] float gravity = 5;
    [Tooltip("If the value is less than 1 then we will use the Mover turn speed, noting that, if its too low, character may not" +
        "\nface the target, by the end of the animation, but the ball, will still travel towards the target")]

    [Header("Read Only")]
    [SerializeField] bool isThrowing = false;
    [SerializeField] string activityName = "Throw Action";

    DodgeballCharacter chara = null;
    Animator animator = null;
    ActionsScheduler scheduler = null;
    DodgeballCharacter aimedAtChara = null;
    Mover mover = null;
    Vector3 lastInput = new Vector3();

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
            Debug.LogWarning("Passing To Friendlies has not been implemented yet");
        }
        else
        {
            animator.SetBool("HasBall", false);
            onThrowPointReached?.Invoke();
            isThrowing = false;

            Vector3 vel = Extentions.GetLaunchVelocity(Dodgeball.instance.position,
                                                        aimedAtChara.ShootablePoint.position, heigth, -gravity);
            //float f =Extentions.GetTravelTime(chara.BallGrabPoint.position, toChara.ShootablePoint.position, vel, Vector3.zero);
            //this.InvokeDelayed(f, () => { Debug.Log("Finished?"); Debug.Break(); });
            Dodgeball.GoLaunchTo(aimedAtChara, vel, Vector3.down * gravity, null);
        }
    }
    public void UpdateInput(Vector3 i)
    {
        this.lastInput = i;
    }
    public void A_OnThrowEnded()
    {
        mover.ReadFacingValues();
        mover.UpdateInput(lastInput);
    }
}