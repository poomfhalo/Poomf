using System;
using DG.Tweening;
using UnityEngine;

public class Dodger : DodgeballCharaAction, ICharaAction,IEnergyAction
{
    [Serializable]
    class DodgeAnimData
    {
        public AnimationClip anim = null;
        public float speed = 1;
        public float GetTime()
        {
            //x = vt
            //t = x/v
            float time = 0;
            time = anim.length / speed;
            return time;
        }
    }
    public bool IsDodging => isDodging;
    public string actionName => "Dodge Action";

    public Action<int> ConsumeEnergy { get; set; }
    public Func<int, bool> CanConsumeEnergy { get; set; }
    public Func<bool> AllowRegen => () => !IsDodging;

    [SerializeField] int energyCost = 40;
    [Header("Dodge Data")]
    [SerializeField] bool playRndDodge = true;
    [Tooltip("If playRndDodge is true, this will be randomly set, otherwise, we will dodge using this animation only")]
    [SerializeField] int dodgeToPlay = 0;
    [Header("Dodge Data")]
    [SerializeField] float dist = 3;
    [Tooltip("WARNING: Animations, of dodging, they must be in the same " +
        "order they're placed at in the animator, or very unexpected behavior will happen")]
    [SerializeField] DodgeAnimData[] anims = new DodgeAnimData[2];
    [SerializeField] Ease ease = Ease.InOutSine;

    [Header("Read Only")]
    [SerializeField] bool isDodging = false;

    Animator animator = null;
    ActionsScheduler scheduler = null;
    Rigidbody rb3d = null;
    Tweener activeTween = null;
    DodgeballCharacter chara = null;

    void Start()
    {
        animator = GetComponent<Animator>();
        scheduler = GetComponent<ActionsScheduler>();
        rb3d = GetComponent<Rigidbody>();
        chara = GetComponent<DodgeballCharacter>();
    }

    public void StartDodgeAction()
    {
        StartDodgeAction(GetExpectedPosition(), null);
    }
    public void StartDodgeAction(Vector3 targetPos,Action onComplete)
    {
        if (!CanConsumeEnergy(energyCost))
            return;

        ConsumeEnergy(energyCost);

        if (playRndDodge)
        {
            dodgeToPlay = UnityEngine.Random.Range(0, anims.Length);
        }
        animator.SetInteger("RndAnim", dodgeToPlay);
        animator.SetTrigger("Dodge");
        scheduler.StartAction(this, true);
        isDodging = true;

        Vector3 startPos = rb3d.position;
        float time = anims[dodgeToPlay].GetTime();
        float lerper = 0;
        activeTween = DOTween.To(() => lerper, f => lerper = f, 1, time).SetEase(ease).OnUpdate(OnUpdate).OnComplete(OnComplete);

        void OnUpdate()
        {
            Vector3 smoothPos = Vector3.Lerp(startPos, targetPos, lerper);
            transform.position = smoothPos;
        }
        void OnComplete()
        {
            onComplete?.Invoke();
        }
    }

    public Vector3 GetExpectedPosition()
    {
        Vector3 expectedPos = rb3d.position + transform.forward * dist;
        return expectedPos;
    }
    public void Cancel()
    {
        activeTween.Kill();
    }
    public void A_OnDodgeCompleted()
    {
        isDodging = false;
        chara.C_MoveInput(recievedInput);
        Cancel();
    }
}