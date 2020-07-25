using System;
using DG.Tweening;
using GW_Lib;
using UnityEngine;

[RequireComponent(typeof(ActionsScheduler))]
public class Mover : DodgeballCharaAction, ICharaAction
{
    public bool IsMoving => isMoving;

    public enum MovementType { ByInput, ToPoint }
    public MovementType movementType
    {
        set
        {
            m_movementType = value;
            Cancel();
        }
        get
        {
            return m_movementType;
        }
    }
    public Func<Vector3> GetYDisp = () => Vector3.zero;

    [SerializeField] float accel = 10;
    [SerializeField] float maxSpeed = 3;


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
    [SerializeField] Vector3 xzVel = Vector3.zero;
    [SerializeField] Vector3 lastNonZeroVel = Vector3.zero;
    [SerializeField] float minInputTimeCounter = 0;
    [SerializeField] int movabilityDir = 0;
    [SerializeField] Vector3 smoothMoveInput = Vector3.zero;
    [SerializeField] float distToLastPos = 0;
    [SerializeField] MovementType m_movementType = MovementType.ByInput;
    [SerializeField] bool isMoving = false;
    public bool workAsAction = true;

    Vector3 dir = Vector3.zero;

    Animator animator
    {
        get
        {
            if (m_animator == null)
                m_animator = GetComponent<Animator>();
            return m_animator;
        }
    }
    Rigidbody rb3d 
    { 
        get
        {
            if (m_rb3d == null)
                m_rb3d = GetComponent<Rigidbody>();
            return m_rb3d;
        }
    }
    Animator m_animator = null;
    Rigidbody m_rb3d = null;
    ActionsScheduler scheduler = null;

    public string actionName => "Move Action";
    Vector3 right = new Vector3(), fwd = new Vector3();
    bool isGoingToMove => movabilityDir == 1;
    bool isGoingToStop => movabilityDir == -1;

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.GetComponent<Dodgeball>())
            rb3d.velocity = Vector3.zero;
    }
    void OnEnable()
    {
        scheduler = GetComponent<ActionsScheduler>();
        ReadFacingValues();
    }
    void Update()
    {
        if (!IsMoving)
            return;

        if (usesInputDelay && movementType == MovementType.ByInput)
        {
            bool isZero = Mathf.Abs(minInputTimeCounter) < Mathf.Epsilon;

            if (isGoingToStop && isZero)
                return;
            if (isGoingToMove && minInputTimeCounter < 1)
                return;
        }

        switch (movementType)
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

        if (usesInputDelay && movementType == MovementType.ByInput)
        {
            minInputTimeCounter = minInputTimeCounter + Time.fixedDeltaTime / minInputTime * movabilityDir;
            minInputTimeCounter = Mathf.Clamp(minInputTimeCounter, 0, 1);

            if (isGoingToStop && minInputTimeCounter < 0)
                return;
            if (isGoingToMove && minInputTimeCounter < 1)
                return;
        }

        if (!IsMoving)
        {
            ApplyVel();
            return;
        }

        SetMoveDir();
        SetVel();
        TurnToDir(lastNonZeroVel, turningSpeed);
        ApplyVel();
    }
    private void ApplyVel()
    {
        Vector3 xzDisp = xzVel * Time.fixedDeltaTime;
        Vector3 yDisp = GetYDisp();
        rb3d.MovePosition(rb3d.position + xzDisp + yDisp);
        rb3d.velocity = Vector3.zero;
    }
    public void StartMoveByInput(Vector3 newInput, Transform withRespectTo)
    {
        if (workAsAction)
        {
            scheduler.StartAction(this);
        }
        ApplyInput(newInput, withRespectTo);
    }
    public void Cancel()
    {
        isMoving = false;
        xzVel = Vector3.zero;
        speed = 0;
        ApplyVel();
        animator.SetFloat("Speed", speed);
        recievedInput = Vector3.zero;
    }
    public void SmoothStop(Action onCompleted)
    {
        if(cancelationTime<=0)
        {
            Cancel();
            onCompleted?.Invoke();
            return;
        }

        isMoving = false;
        Vector3 cancelStartVel = xzVel;
        float startCancelSpeed = speed;

        float f = 0;
        DOTween.To(() => f, (newF) => f = newF, 1, cancelationTime).SetEase(Ease.Linear).OnUpdate(() =>
        {
            xzVel = Vector3.Slerp(cancelStartVel, Vector3.zero, f);
            ApplyVel();
            speed = Mathf.Lerp(startCancelSpeed, 0, f);
            animator.SetFloat("Speed", speed);
        }).OnComplete(() =>
        {
            Cancel();
            onCompleted?.Invoke();
        });
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
        switch (movementType)
        {
            case MovementType.ByInput:
                SetVelByInput();
                break;
            case MovementType.ToPoint:
                SetVelByPoint();
                break;
        }

        speed = xzVel.magnitude;
        if (speed > dampingSpeed)
        {
            lastNonZeroVel = xzVel;
            xzVel = Vector3.ClampMagnitude(xzVel, maxSpeed);
        }
    }
    private void SetVelByInput()
    {
        if (dir != Vector3.zero)
        {
            xzVel = xzVel + dir * accel * Time.fixedDeltaTime;
        }
        else
        {
            if (speed > dampingSpeed)
            {
                xzVel = xzVel + -xzVel.normalized * deAccel * Time.fixedDeltaTime;
            }
            if (speed < dampingSpeed && speed > stoppingSpeed)
            {
                xzVel = xzVel / speedDamper;
            }
            else if (speed <= stoppingSpeed && IsMoving)
            {
                xzVel = Vector3.zero;
                if (isGoingToStop)
                    Cancel();
            }
        }
    }
    private void SetVelByPoint()
    {
        if (distToLastPos > slowingDist)
        {
            xzVel = xzVel + dir * accel * Time.fixedDeltaTime;
        }
        else if (distToLastPos < slowingDist && distToLastPos > stoppingDist)
        {
            float distFactor = distToLastPos / slowingDist;
            xzVel = dir * distFactor * maxSpeed;
        }
        else if (distToLastPos < stoppingDist)
        {
            xzVel = Vector3.zero;
            Cancel();
        }
    }
    private void SetMoveDir()
    {
        dir = Vector3.zero;
        switch (movementType)
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
    private void ApplyInput(Vector3 newInput, Transform withRespectTo)
    {
        if (withRespectTo == null)
        {
            right = Vector3.right;
            fwd = Vector3.forward;
        }
        else
        {
            //right = withRespectTo.right;
            //fwd = withRespectTo.forward;

            right = Extentions.GetClosestFacingV(withRespectTo.right, null);
            fwd = Extentions.GetClosestFacingV(withRespectTo.forward, null);
        }
        ApplyInput(newInput);
    }
    public void ApplyInput(Vector3 input)
    {
        isMoving = true;
        this.recievedInput = input;
        movabilityDir = 1;
        if (input == Vector3.zero && movementType == MovementType.ByInput)
        {
            movabilityDir = -1;
        }
    }
    #endregion
}