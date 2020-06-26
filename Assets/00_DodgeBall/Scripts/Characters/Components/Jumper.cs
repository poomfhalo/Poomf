using GW_Lib;
using UnityEngine;

public class Jumper : DodgeballCharaAction, ICharaAction
{
    public bool FeelsGround => feelsGround;
    public bool IsJumping => isJumping;
    public string actionName => "Jump Action";

    [SerializeField] float gravity = -20;
    [SerializeField] float jumpHeigth = 3;
    [SerializeField] float timeBeforeReapplyGravity = 0.1f;
    [SerializeField] float yStopping = 0.0250f;
    [Header("Ground Detection")]
    [SerializeField] float castDist = 0.2f;
    [SerializeField] Transform feet = null;
    [Header("Read Only")]
    [SerializeField] bool isJumping = false;
    [SerializeField] bool feelsGround = false;
    [SerializeField] float currYVel = 0;

    Animator animator = null;
    ActionsScheduler scheduler = null;
    Mover mover = null;
    Rigidbody rb3d = null;

    RaycastHit hit;
    Ray ray = new Ray();
    bool canApplyGravity = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
        scheduler = GetComponent<ActionsScheduler>();
        mover = GetComponent<Mover>();
        rb3d = GetComponent<Rigidbody>();
        mover.GetYDisp = GetYDisp;
    }

    private Vector3 GetYDisp()
    {
        if (canApplyGravity && FeelsGround)
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

    void FixedUpdate()
    {
        ApplyGroundTest();
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        if(!canApplyGravity)
        {
            return;
        }

        if (FeelsGround)
        {
            currYVel = gravity * Time.fixedDeltaTime;
        }
        else
        {
            currYVel = currYVel + gravity * Time.fixedDeltaTime;
        }
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
    }

    public void StartJumpAction()
    {
        if (!FeelsGround)
            return;
        if (IsJumping)
            return;

        isJumping = true;
        animator.SetTrigger("Jump");
        scheduler.StartAction(this, false);
        float jumpVel = Extentions.GetJumpVelocity(jumpHeigth,gravity);
        currYVel = jumpVel;
        canApplyGravity = false;
        this.InvokeDelayed(timeBeforeReapplyGravity, () => canApplyGravity = true);
    }
    public void Cancel()
    {

    }
    public void A_OnJumpEnded()
    {
        isJumping = false;
    }
    public override void RecieveInput(Vector3 i)
    {
        base.RecieveInput(i);

        if (!IsJumping)
        {
            return;
        }
        mover.ApplyInput(i);
    }
}