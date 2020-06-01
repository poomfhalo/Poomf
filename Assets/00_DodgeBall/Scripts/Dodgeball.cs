using GW_Lib.Utility;
using UnityEngine;
using DG.Tweening;
using System;
using GW_Lib;

public class Dodgeball : Singleton<Dodgeball>
{
    public Vector3 position { get { return transform.position; } set { transform.position = value; } }

    [Tooltip("Time, before ball can collide again, with physics colliders after being thrown")]
    [SerializeField] float leavingHandsTime = 0.1f;
    [SerializeField] float defGrabTime = 0.1f;
    [SerializeField] Collider bodyCol = null;
    [Header("Read Only")]
#pragma warning disable RECS0122 // Initializing field with default value is redundant
    [SerializeField] bool isCaught = false;
#pragma warning restore RECS0122 // Initializing field with default value is redundant
    [SerializeField] DodgeballCharacter holder = null;
    [SerializeField] float currTweener = 0;

    Rigidbody rb3d = null;
    ConstantForce cf = null;
    Vector3 startGravity = Vector3.zero;

    protected override void Awake()
    {
        base.Awake();
        rb3d = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
        startGravity = cf.force;
        rb3d.useGravity = false;
    }

    private static void SetHolder(DodgeballCharacter chara)
    {
        instance.isCaught = true;
        instance.holder = chara;
    }

    public static void GoLaunchTo(DodgeballCharacter chara,Vector3 launchVel,Vector3 gravity, Action onCompleted)
    {
        instance.rb3d.isKinematic = false;
        instance.rb3d.collisionDetectionMode = CollisionDetectionMode.Continuous;
        instance.InvokeDelayed(instance.leavingHandsTime, () => instance.bodyCol.enabled = true);
        instance.rb3d.velocity = launchVel;
        instance.cf.force = gravity;
    }

    public static void GoTo(DodgeballCharacter chara, Action onCompleted ,float grabTime = -1)
    {
        SetHolder(chara);
        float dur = grabTime;

        if (grabTime < 0)
            dur = instance.defGrabTime;

        instance.bodyCol.enabled = false;
        instance.rb3d.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        instance.rb3d.isKinematic = true;

        Vector3 startPos = instance.position;
        DOTween.To(instance.Getter, instance.Setter, 1, dur).OnUpdate(OnUpdate).OnComplete(OnComplete).SetEase(Ease.InOutSine);

        void OnUpdate()
        {
             instance.position = Vector3.Lerp(startPos, chara.BallGrabPoint.position, instance.currTweener);
        }
        void OnComplete()
        {
            onCompleted?.Invoke();
            instance.currTweener = 0;
            instance.transform.SetParent(chara.BallGrabPoint);
        }
    }

    void Setter(float pNewValue)
    {
        instance.currTweener = pNewValue;
    }
    float Getter()
    {
        return instance.currTweener;
    }

    void OnDestroy()
    {
        isCaught = false;
        holder = null; 
        currTweener = 0;
    }
}