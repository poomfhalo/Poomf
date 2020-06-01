using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ActionsScheduler))]
public class Mover : MonoBehaviour,ICharaAction
{
    public bool IsMoving => currState != MovementState.Stopped;
    enum MovementState { Stopped,ByInput,ToPoint }
    [SerializeField] MovementState currState = MovementState.Stopped;
    [SerializeField] float accel = 10;
    [SerializeField] float maxSpeed = 3;

    [Header("Slowing Down")]
    [Tooltip("how fast speed decreases, when no input is being applied")]
    [SerializeField] float deAccel = 5;
    [Tooltip("speed of character, before he starts slowing down")]
    [SerializeField] float dampingSpeed = 0.1f;
    [Tooltip("value of which speed is divide by, when we are slowing down (should be higher than 1)")]
    [Range(1.01f,10f)]
    [SerializeField] float speedDamper = 1.5f;
    [Tooltip("speed of character before he snaps to zero")]
    [SerializeField] float stoppingSpeed = 0.01f;
    [Tooltip("If we enter an action, how long it will take the character to stop in seconds")]
    [SerializeField] float cancelationTime = 0.1f;

    [Header("Turning")]
    [SerializeField] float turningSpeed = 290;

    [Header("Read Only")]
    [SerializeField] float speed = 0;
    [SerializeField] Vector3 lastNonZeroDir = Vector3.zero;
    [SerializeField] Vector3 vel = Vector3.zero;
    [SerializeField] Vector3 lastNonZeroVel = Vector3.zero;

    Vector3 input = Vector3.zero;
    Vector3 dir = Vector3.zero;

    Transform currMovePoint = null;

    Animator animator = null;
    Rigidbody rb3d = null;
    ActionsScheduler scheduler = null;

    public string actionName => "Move Action";
    Vector3 right = new Vector3(), fwd = new Vector3();

    void OnEnable()
    {
        animator = GetComponent<Animator>();
        rb3d = GetComponent<Rigidbody>();
        scheduler = GetComponent<ActionsScheduler>();

        lastNonZeroDir = transform.forward;
        lastNonZeroVel = lastNonZeroDir * (stoppingSpeed + 0.05f);
    }
    void Update()
    {
        if (!IsMoving)
            return;
        animator.SetFloat("Speed", speed);
    }
    void FixedUpdate()
    {
        if (!IsMoving)
            return;
        SetMoveDir();
        SetVel();
        TurnToDir(lastNonZeroVel,turningSpeed);
    }

    public void StartMoveByInput(Vector3 newInput, Transform withRespectTo)
    {
        scheduler.StartAction(this);
        UpdateInput(newInput, withRespectTo);
    }
    public void StartMoveTo(Transform point)
    {
        scheduler.StartAction(this);
        MoveTo(point);
    }
    public void Cancel()
    {
        currState = MovementState.Stopped;
        currMovePoint = null;
        vel = Vector3.zero;
        speed = 0;
        rb3d.velocity = vel;
        animator.SetFloat("Speed", speed);
    }
    public void SmoothStop(Action onCompleted)
    {
        currState = MovementState.Stopped;
        currMovePoint = null;
        Vector3 cancelStartVel = vel;
        float startCancelSpeed = speed;

        float f = 0;
        DOTween.To(() => f, (newF) => f = newF, 1, cancelationTime).SetEase(Ease.InOutSine).OnUpdate(() => {
            rb3d.velocity = Vector3.Slerp(cancelStartVel, Vector3.zero, f);
            speed = Mathf.Lerp(startCancelSpeed, 0, f);
            animator.SetFloat("Speed", speed);
        }).OnComplete(() => {
            rb3d.velocity = Vector3.zero;
            speed = 0;
            animator.SetFloat("Speed", speed);
            onCompleted?.Invoke();
        });
    }

    public void MoveTo(Transform point)
    {
        currState = MovementState.ToPoint;
        this.currMovePoint = point;
    }
    public void UpdateInput(Vector3 newInput,Transform withRespectTo)
    {
        currState = MovementState.ByInput;
        this.input = newInput;
        if(withRespectTo == null)
        {
            right = Vector3.right;
            fwd = Vector3.forward;
        }
        else
        {
            right = withRespectTo.right;
            fwd = withRespectTo.forward;
        }
    }

    public void TurnToPoint(Vector3 pos, float turnSpeed)
    {
        TurnToDir(pos - rb3d.position,turnSpeed);
    }
    public void TurnToPoint(Vector3 pos)
    {
        TurnToDir(pos - rb3d.position, turningSpeed);
    }

    private void TurnToDir(Vector3 facingDir,float turnSpeed)
    {
        if (turnSpeed < 1)
            turnSpeed = this.turningSpeed;
        Quaternion targetRot = Quaternion.LookRotation(facingDir);
        Quaternion smoothRot = Quaternion.RotateTowards(transform.rotation, targetRot, turningSpeed * Time.fixedTime);
        rb3d.MoveRotation(smoothRot);
    }
    private void SetVel()
    {
        if (dir != Vector3.zero)
        {
            vel = vel + dir * accel * Time.fixedDeltaTime;
        }
        else
        {
            if (speed > dampingSpeed)
            {
                vel = vel + -vel.normalized * deAccel * Time.fixedDeltaTime;
            }
            if (speed < dampingSpeed && speed > stoppingSpeed)
            {
                vel = vel / speedDamper;
            }
            else if (speed <= stoppingSpeed && IsMoving)
            {
                vel = Vector3.zero;
                Cancel();
            }
        }
        speed = vel.magnitude;
        if(speed>dampingSpeed)
        {
            lastNonZeroVel = vel;
            vel = Vector3.ClampMagnitude(vel, maxSpeed);
        }
        rb3d.velocity = vel;
    }
    private void SetMoveDir()
    {
        dir = Vector3.zero;

        switch (currState)
        {
            case MovementState.ByInput:
                if (Mathf.Abs(input.x) > 0)
                {
                    dir = dir + right * Mathf.Sign(input.x);
                }
                if (Mathf.Abs(input.z) > 0)
                {
                    dir = dir + fwd * Mathf.Sign(input.z);
                }
                dir.y = 0;
                break;
            case MovementState.ToPoint:
                if (currMovePoint != null)
                    dir = (currMovePoint.position - rb3d.position).normalized;
                break;
        }

        if (dir == Vector3.zero)
            return;
        lastNonZeroDir = dir;
        dir.Normalize();
    }
}