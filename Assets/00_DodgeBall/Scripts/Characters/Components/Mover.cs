using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(ActionsScheduler))]
public class Mover : DodgeballCharaAction, ICharaAction
{
    //Note: Y vel is directly changed by gravity
    //this script, only modifies the XZVel for the characters.
    public Vector3 YVel
    {
        get
        {
            Vector3 yVel = vel;
            yVel.x = yVel.z = 0;
            return yVel;
        }
        set
        {
            Vector3 oldVel = vel;
            Vector3 newVel = value;
            newVel.x = oldVel.x;
            newVel.z = oldVel.z;
            vel = newVel;
        }
    }
    private Vector3 XZVel
    {
        get
        {
            Vector3 xzVel = vel;
            xzVel.y = 0;
            return xzVel;
        }
        set
        {
            Vector3 oldVel = vel;
            Vector3 newVel = value;
            newVel.y = oldVel.y;
            vel = newVel;
        }
    }

    public float GetGravity => cf.force.y;

    public enum MovementType { ByInput, ToPoint }
    public MovementType movementMode = MovementType.ByInput;
    [SerializeField] float accel = 10;
    public float maxSpeed = 3;
    [SerializeField] float gravity = -20;
    public bool IsMoving = false;

    [Header("Slowing Down")]
    [Tooltip("how fast speed decreases, when no input is being applied")]
    [SerializeField] float deAccel = 5;
    [Tooltip("speed of character, before he starts slowing down")]
    [SerializeField] float dampingSpeed = 0.1f;
    [Tooltip("value of which speed is divide by, when we are slowing down (should be higher than 1)")]
    [Range(1.01f, 10f)]
    [SerializeField] float speedDamper = 1.5f;
    [Tooltip("speed of character before he snaps to zero")]
    [SerializeField] float stoppingSpeed = 0.01f;
    [Tooltip("If we enter an action, how long it will take the character to stop in seconds")]
    [SerializeField] float cancelationTime = 0.1f;

    [Header("Move To Point Data")]
    [SerializeField] float slowingDist = 0.5f;
    [SerializeField] float stoppingDist = 0.11f;
    [Header("Turning")]
    [SerializeField] float turningSpeed = 290;

    [Header("Input Saftey")]
    [SerializeField] float minInputTime = 0.08f;
    [SerializeField] float inputSensitivity = 3f;
    [SerializeField] float minMoveInput = 0.325f;
    [SerializeField] bool usesInputDelay = true;

    [Header("Read Only")]
    [SerializeField] float speed = 0;
    [SerializeField] Vector3 vel = Vector3.zero;
    [SerializeField] Vector3 lastNonZeroVel = Vector3.zero;
    [SerializeField] float minInputTimeCounter = 0;
    [SerializeField] int movabilityDir = 0;
    [SerializeField] Vector3 smoothMoveInput = Vector3.zero;
    [SerializeField] float distToLastPos = 0;

    Vector3 dir = Vector3.zero;

    Animator animator = null;
    Rigidbody rb3d = null;
    ActionsScheduler scheduler = null;
    ConstantForce cf = null;

    public string actionName => "Move Action";
    Vector3 right = new Vector3(), fwd = new Vector3();
    bool isGoingToMove => movabilityDir == 1;
    bool isGoingToStop => movabilityDir == -1;

    void OnEnable()
    {
        animator = GetComponent<Animator>();
        rb3d = GetComponent<Rigidbody>();
        rb3d.useGravity = false;
        scheduler = GetComponent<ActionsScheduler>();
        cf = GetComponent<ConstantForce>();
        cf.force = Vector3.up * gravity;

        ReadFacingValues();
    }
    void Update()
    {
        if (!IsMoving)
            return;

        if (usesInputDelay && movementMode == MovementType.ByInput)
        {
            bool isZero = Mathf.Abs(minInputTimeCounter) < Mathf.Epsilon;

            if (isGoingToStop && isZero)
                return;
            if (isGoingToMove && minInputTimeCounter < 1)
                return;
        }

        switch (movementMode)
        {
            case MovementType.ByInput:
                animator.SetFloat("Speed", speed);
                break;
            case MovementType.ToPoint:
                float s = 0;
                if (distToLastPos > stoppingDist)
                {
                    s = maxSpeed;
                }
                animator.SetFloat("Speed", s);
                break;
        }
    }
    void FixedUpdate()
    {
        smoothMoveInput = Vector3.MoveTowards(smoothMoveInput, recievedInput, inputSensitivity * Time.fixedDeltaTime);

        if (usesInputDelay && movementMode == MovementType.ByInput)
        {
            minInputTimeCounter = minInputTimeCounter + Time.fixedDeltaTime / minInputTime * movabilityDir;
            minInputTimeCounter = Mathf.Clamp(minInputTimeCounter, 0, 1);

            if (isGoingToStop && minInputTimeCounter < 0)
                return;
            if (isGoingToMove && minInputTimeCounter < 1)
                return;
        }

        if (!IsMoving)
            return;

        SetMoveDir();
        SetVel();
        TurnToDir(lastNonZeroVel, turningSpeed);

        if(rb3d.isKinematic)
        {
            rb3d.MovePosition(rb3d.position + vel * Time.fixedDeltaTime);
        }
        else
        {
            rb3d.velocity = vel;
        }
    }

    public void StartMoveByInput(Vector3 newInput, Transform withRespectTo)
    {
        scheduler.StartAction(this);
        ApplyInput(newInput, withRespectTo);
    }
    public void Cancel()
    {
        IsMoving = false;
        vel = Vector3.zero;
        speed = 0;
        XZVel = vel;
        animator.SetFloat("Speed", speed);
    }
    public void SmoothStop(Action onCompleted)
    {
        IsMoving = false;
        Vector3 cancelStartVel = vel;
        float startCancelSpeed = speed;

        float f = 0;
        DOTween.To(() => f, (newF) => f = newF, 1, cancelationTime).SetEase(Ease.InOutSine).OnUpdate(() =>
        {
            XZVel = Vector3.Slerp(cancelStartVel, Vector3.zero, f);
            speed = Mathf.Lerp(startCancelSpeed, 0, f);
            animator.SetFloat("Speed", speed);
        }).OnComplete(() =>
        {
            XZVel = Vector3.zero;
            speed = 0;
            animator.SetFloat("Speed", speed);
            onCompleted?.Invoke();
        });

    }
    public void Warp(Vector3 warpPos)
    {
        transform.position = warpPos;
        speed = 0;
        animator.SetFloat("Speed", speed);
        XZVel = Vector3.zero;
        recievedInput = Vector3.zero;
        ReadFacingValues();
    }
    public void TurnToPoint(Vector3 pos, float turnSpeed)
    {
        TurnToDir(pos - rb3d.position, turnSpeed);
    }
    public void TurnToPoint(Vector3 pos)
    {
        TurnToDir(pos - rb3d.position, turningSpeed);
    }
    private void TurnToDir(Vector3 facingDir, float turnSpeed)
    {
        if (turnSpeed < 1)
            turnSpeed = this.turningSpeed;
        Quaternion targetRot = Quaternion.LookRotation(facingDir);
        Quaternion smoothRot = Quaternion.RotateTowards(transform.rotation, targetRot, turningSpeed * Time.fixedTime);
        rb3d.MoveRotation(smoothRot);
    }
    private void SetVel()
    {
        switch (movementMode)
        {
            case MovementType.ByInput:
                SetVelByInput();
                break;
            case MovementType.ToPoint:
                SetVelByPoint();
                break;
        }

        speed = vel.magnitude;
        if (speed > dampingSpeed)
        {
            lastNonZeroVel = vel;
            vel = Vector3.ClampMagnitude(vel, maxSpeed);
        }
        XZVel = vel;
    }
    private void SetVelByInput()
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
                if (isGoingToStop)
                    Cancel();
            }
        }
    }
    private void SetVelByPoint()
    {
        if (distToLastPos > slowingDist)
        {
            vel = vel + dir * accel * Time.fixedDeltaTime;
        }
        else if (distToLastPos < slowingDist && distToLastPos > stoppingDist)
        {
            float distFactor = distToLastPos / slowingDist;
            vel = dir * distFactor * maxSpeed;
        }
        else if (distToLastPos < stoppingDist)
        {
            vel = Vector3.zero;
            Cancel();
        }
    }
    private void SetMoveDir()
    {
        dir = Vector3.zero;
        switch (movementMode)
        {
            case MovementType.ByInput:
                float ax = Mathf.Abs(smoothMoveInput.x);
                if (ax > 0)
                {
                    float usableX = ax <= minMoveInput ? 0 : smoothMoveInput.x;
                    dir = dir + right * usableX;
                }
                float az = Mathf.Abs(smoothMoveInput.z);
                if (az > 0)
                {
                    float usableZ = az <= minMoveInput ? 0 : smoothMoveInput.z;
                    dir = dir + fwd * usableZ;
                }
                dir.y = 0;
                break;
            case MovementType.ToPoint:
                dir = recievedInput - rb3d.position;
                dir.y = 0;
                distToLastPos = dir.magnitude;
                break;
        }

        if (dir == Vector3.zero)
            return;

        dir.Normalize();
    }

    #region ApplyInput
    public void ReadFacingValues()
    {
        lastNonZeroVel = transform.forward * (stoppingSpeed + 0.05f);
    }
    public void ApplyInput(Vector3 newInput, Transform withRespectTo)
    {
        if (withRespectTo == null)
        {
            right = Vector3.right;
            fwd = Vector3.forward;
        }
        else
        {
            right = withRespectTo.right;
            fwd = withRespectTo.forward;
        }
        ApplyInput(newInput);
    }
    public void ApplyInput(Vector3 input)
    {
        IsMoving = true;
        this.recievedInput = input;
        movabilityDir = 1;
        if (input == Vector3.zero && movementMode == MovementType.ByInput)
        {
            movabilityDir = -1;
        }
    }
    #endregion
}