using GW_Lib;
using UnityEngine;

public class Jumper : MonoBehaviour, ICharaAction
{
    public bool FeelsGround => feelsGround;
    public bool IsJumping => isJumping;
    public string actionName => "Jump Action";

    [SerializeField] float jumpHeigth = 3;
    [Header("Ground Detection")]
    [SerializeField] float castDist = 0.2f;
    [SerializeField] Transform feet = null;
    [Header("Read Only")]
    [SerializeField] bool isJumping = false;
    [SerializeField] bool feelsGround = false;

    Animator animator = null;
    ActionsScheduler scheduler = null;
    Mover mover = null;
    Rigidbody rb3d = null;

    RaycastHit hit;
    Ray ray = new Ray();

    void Awake()
    {
        animator = GetComponent<Animator>();
        scheduler = GetComponent<ActionsScheduler>();
        mover = GetComponent<Mover>();
        rb3d = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        feelsGround = false;

        ray.origin = feet.position;
        ray.direction = Vector3.down;

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
        float jumpVel = Extentions.GetJumpVelocity(jumpHeigth,mover.GetGravity);
        mover.YVel = Vector3.up * jumpVel;
    }

    public void Cancel()
    {

    }

    public void A_OnJumpEnded()
    {
        isJumping = false;
    }
    public void UpdateInput(Vector3 i)
    {
        mover.UpdateInput(i);
    }
}