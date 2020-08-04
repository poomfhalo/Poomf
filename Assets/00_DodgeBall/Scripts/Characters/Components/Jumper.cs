using System;
using System.Collections.Generic;
using System.Linq;
using GW_Lib;
using UnityEngine;

public class Jumper : DodgeballCharaAction, ICharaAction,IEnergyAction
{
    public bool FeelsGround => feelsGround;
    public bool IsJumping => isJumping;
    public string actionName => "Jump Action";
    public float PosToJumpHeigthPercent => posToJumpHeigthPercent;

    public Action<int> ConsumeEnergy { get; set; }
    public Func<int, bool> CanConsumeEnergy { get; set; }
    public Func<bool> AllowRegen => () => !IsJumping && FeelsGround;

    [SerializeField] int energyCost = 20;

    [Header("Jump Data")]
    [SerializeField] float gravity = -20;
    [SerializeField] float jumpHeigth = 3;
    [SerializeField] float yStopping = 0.0250f;
    [Header("Ground Detection")]
    [SerializeField] float castDist = 0.2f;
    [SerializeField] Transform feet = null;

    [Header("Read Only")]
    [SerializeField] bool isJumping = false;
    [SerializeField] bool feelsGround = false;
    [SerializeField] float currYVel = 0;
    [SerializeField] float posToJumpHeigthPercent = 0;
    [SerializeField] float floorHeigthRef = -1;
    [SerializeField] bool isGoingUp = false;

    Animator animator = null;
    ActionsScheduler scheduler = null;
    Mover mover = null;
    Rigidbody rb3d = null;
    BallLauncher launcher = null;

    RaycastHit hit;
    Ray ray = new Ray();
    bool heigthRefWasSet = false;
    bool canApplyGravity = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
        scheduler = GetComponent<ActionsScheduler>();
        mover = GetComponent<Mover>();
        rb3d = GetComponent<Rigidbody>();
        launcher = GetComponents<BallLauncher>().ToList().Find(b => b.enabled);

        mover.GetYDisp = GetYDisp;
    }
    void FixedUpdate()
    {
        UpdateHeigthPercent();
        ApplyGroundTest();
        ApplyGravity();
    }

    private void UpdateHeigthPercent()
    {
        if(!heigthRefWasSet)
        {
            List<RaycastHit> hs = Physics.RaycastAll(ray, 100).ToList();
            if (hs.Count== 0)
                return;
            RaycastHit h = hs.Find(sampleHit => sampleHit.collider.GetComponent<Field>());
            if (!h.collider)
                return;

            floorHeigthRef = h.point.y;
            heigthRefWasSet = true;
            return;
        }

        float diff = Mathf.Abs(rb3d.position.y - floorHeigthRef);
        posToJumpHeigthPercent = 1 - (diff / jumpHeigth);
    }

    public void StopFlight()
    {
        canApplyGravity = false;
        Debug.Log("Stopping Flight");
        currYVel = 0;
    }
    public void ResumeFlight()
    {
        canApplyGravity = true;
        Debug.Log("Resuming Flight");
    }

    private Vector3 GetYDisp()
    {
        if (FeelsGround && !isGoingUp)
        {
            Vector3 yDisp = Vector3.zero;
            yDisp = hit.point - rb3d.position;
            yDisp.z = yDisp.x = 0;
            if (yDisp.magnitude <= yStopping)
            {
                return Vector3.zero;
            }
            return yDisp;
        }

        return currYVel * Time.fixedDeltaTime * Vector3.up;
    }
    private void ApplyGravity()
    {
        if (!canApplyGravity)
            return;

        if (currYVel <= 0)
            isGoingUp = false;

        if (FeelsGround && !isGoingUp)
        {
            currYVel = gravity * Time.fixedDeltaTime;
            animator.ResetTrigger("Jump");
        }
        else
            currYVel = currYVel + gravity * Time.fixedDeltaTime;
    }
    private void ApplyGroundTest()
    {
        feelsGround = false;

        ray.origin = feet.position;
        ray.direction = Vector3.down;
        Debug.DrawRay(ray.origin, ray.direction * castDist, Color.red);

        if (!Physics.Raycast(ray, out hit, castDist))
            return;
        Field field = hit.collider.GetComponent<Field>();
        if (!field)
            return;

        feelsGround = true;
        if (isJumping && feelsGround && !isGoingUp)
            isJumping = false;
    }

    public void StartJumpAction()
    {
        if (!FeelsGround)
            return;
        if (IsJumping)
            return;
        if (CanConsumeEnergy != null && !CanConsumeEnergy(energyCost))
            return;

        ConsumeEnergy?.Invoke(energyCost);
        isJumping = true;
        animator.SetTrigger("Jump");
        scheduler.StartAction(this, false);
        float jumpVel = Extentions.GetJumpVelocity(jumpHeigth,gravity);
        currYVel = jumpVel;
        isGoingUp = true;
    }
    public void Cancel(){ }
    public void A_OnJumpEnded()
    {
        isJumping = false;
        animator.ResetTrigger("Jump");
    }

    public override void RecieveInput(Vector3 i)
    {
        base.RecieveInput(i);

        if (!IsJumping)
            return;
        if (launcher.IsThrowing)
            return;

        mover.ApplyInput(i);
    }
}