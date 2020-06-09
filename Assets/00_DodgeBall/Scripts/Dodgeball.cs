using UnityEngine;
using DG.Tweening;
using System;
using GW_Lib;
using GW_Lib.Utility;

//TODO: based on Dodgeball speed, we increase size of body colliders of characters up to 4, so that, the dodge ball
//does not get missed by smaller colliders, when its going too fast.
//TODO: dynamically find the gravity, depending on distance, such that, speed never exceeds a certain value?
public enum DodgeballCommand { GoToChara, LaunchUp, LaunchTo }

public class Dodgeball : Singleton<Dodgeball>
{
    public bool IsOnGround => ballState == BallState.OnGround;
    public bool IsHeld => ballState == BallState.Held;
    public bool IsFlying => ballState == BallState.Flying;
    public bool IsGoingToChara => ballState == BallState.GoingToChara;
    public bool IsPostContact => ballState == BallState.PostContact;

    public event Action<DodgeballCommand> OnCommandActivated = null;

    public enum BallState { OnGround,Held,Flying,GoingToChara,PostContact }

    public Vector3 position { get { return transform.position; } set { transform.position = value; } }

    [Tooltip("Time, before ball can collide again, with physics colliders after being thrown")]
    [SerializeField] float leavingHandsTime = 0.1f;
    [SerializeField] float defGrabTime = 0.1f;
    [SerializeField] CollisionDelegator bodyCol = null;
    [SerializeField] float gameStartLaunchHeigth = 3;

    [Header("Read Only")]
    [SerializeField] bool isCaught = false;
    [SerializeField] DodgeballCharacter holder = null;
    [SerializeField] float currTweener = 0;
    public byte lastAppliedThrow = 0;
    public Vector3 lastTargetPos = new Vector3();
    [Header("Synced Variables")]
    public BallState ballState = BallState.Flying;

    Rigidbody rb3d = null;
    ConstantForce cf = null;
    Vector3 startGravity = Vector3.zero;
    Tweener activeTweener = null;

    public Func<bool> CanApplyLaunchToAction = () => true;
    public Func<bool> CanApplyGoToAction = () => true;

    public DodgeballCharacter GetHolder()
    {
        return holder;
    }

    protected override void Awake()
    {
        base.Awake();
        rb3d = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
        startGravity = cf.force;
        rb3d.useGravity = false;
        bodyCol.onCollisionEnter.AddListener(OnBodyEntered);
        bodyCol.onCollisionEnter.AddListener(OnBodyExitted);
    }
    void OnDestroy()
    {
        isCaught = false;
        holder = null;
        currTweener = 0;
        transform.DOKill();
    }

    void OnTriggerEnter(Collider col)
    {
        Field field = col.GetComponent<Field>();
        if (!field)
            return;
        if(ballState == BallState.Flying || ballState == BallState.PostContact)
        {
            ballState = BallState.OnGround;
        }
    }
    void OnTriggerExit(Collider col)
    {
        Field field = col.GetComponent<Field>();
        if (!field)
            return;

        if (ballState == BallState.OnGround)
        {
            ballState = BallState.Flying;
        }
    }
    private void OnBodyEntered(Collision col)
    {

    }
    private void OnBodyExitted(Collision col)
    {
        //m_ballState = BallState.PostContact;
    }

    public static void GoLaunchTo(Vector3 targetPos, BallThrowData d)
    {
        if (instance.CanApplyLaunchToAction())
        {
            instance.SetKinematic(true);
            instance.InvokeDelayed(instance.leavingHandsTime, () => instance.bodyCol.GetCollider.enabled = true);
            float dist = Vector3.Distance(instance.rb3d.position, targetPos);
            float time = d.GetTimeOfDist(dist);
            float tweener = 0;

            instance.activeTweener = DOTween.To(() => tweener, f => tweener = f, 1, time).SetEase(d.ease).OnUpdate(OnUpdate).OnComplete(OnComplete);
            void OnUpdate()
            {
                Vector3 lerpedPos = Vector3.Lerp(instance.position, targetPos, tweener);
                instance.rb3d.MovePosition(lerpedPos);
            }
            void OnComplete()
            {

            }
        }
        instance.lastTargetPos = targetPos;
        instance.lastAppliedThrow = d.id;
        instance.OnCommandActivated?.Invoke(DodgeballCommand.LaunchTo);
    }
    public static void GoLaunchTo(DodgeballCharacter chara,Vector3 launchVel,Vector3 gravity, Action onCompleted)
    {
        instance.SetKinematic(false);
        instance.InvokeDelayed(instance.leavingHandsTime, () => instance.bodyCol.GetCollider.enabled = true);
        instance.rb3d.velocity = launchVel;
        instance.cf.force = gravity;
        instance.ballState = BallState.Flying;
    }
    public static void GoTo(DodgeballCharacter chara, Action onCompleted ,float grabTime = -1)
    {
        instance.ballState = BallState.GoingToChara;
        float dur = grabTime;

        if (grabTime < 0)
            dur = instance.defGrabTime;

        instance.bodyCol.GetCollider.enabled = false;
        instance.SetKinematic(true);

        Vector3 startPos = instance.position;
        DOTween.To(Getter, Setter, 1, dur).OnUpdate(OnUpdate).OnComplete(OnComplete).SetEase(Ease.InOutSine);

        instance.isCaught = true;
        instance.holder = chara;
        instance.OnCommandActivated?.Invoke(DodgeballCommand.GoToChara);

        void OnUpdate()
        {
             instance.position = Vector3.Lerp(startPos, chara.BallGrabPoint.position, instance.currTweener);
        }
        void OnComplete()
        {
            instance.ballState = BallState.Held;

            onCompleted?.Invoke();
            instance.currTweener = 0;
            instance.transform.SetParent(chara.BallGrabPoint);
        }
        void Setter(float pNewValue)
        {
            instance.currTweener = pNewValue;
        }
        float Getter()
        {
            return instance.currTweener;
        }
    }

    public void LaunchUp(float byHeigth, float launchGravity)
    {
        if (Mathf.Approximately(byHeigth,3))
            byHeigth = gameStartLaunchHeigth;

        cf.force = Vector3.up * launchGravity;
        this.SetKinematic(false);
        float yVel = Extentions.GetJumpVelocity(byHeigth, cf.force.y);
        rb3d.velocity = Vector3.up * yVel;
    }

}