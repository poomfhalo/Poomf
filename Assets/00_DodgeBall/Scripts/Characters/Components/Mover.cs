using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(ActionsScheduler))]
public class Mover : MonoBehaviour, ICharaAction
{
    //Note: Y vel is directly changed by gravity
    //this script, only modifies the XZVel for the characters.
    public Vector3 YVel
    {
        get
        {
            Vector3 yVel = rb3d.velocity;
            yVel.x = yVel.z = 0;
            return yVel;
        }
        set
        {
            Vector3 oldVel = rb3d.velocity;
            Vector3 newVel = value;
            newVel.x = oldVel.x;
            newVel.z = oldVel.z;
            rb3d.velocity = newVel;
        }
    }
    private Vector3 XZVel
    {
        get
        {
            Vector3 xzVel = rb3d.velocity;
            xzVel.y = 0;
            return xzVel;
        }
        set
        {
            Vector3 oldVel = rb3d.velocity;
            Vector3 newVel = value;
            newVel.y = oldVel.y;
            rb3d.velocity = newVel;
        }
    }

    public float GetGravity => cf.force.y;
    public bool IsMoving => currState != MovementState.Stopped;
    enum MovementState { Stopped,ByInput,ToPoint }
    [SerializeField] MovementState currState = MovementState.Stopped;
    [SerializeField] float accel = 10;
    [SerializeField] float maxSpeed = 3;
    [SerializeField] float gravity = -20;

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

    [Header("Input Saftey")]
    [SerializeField] float minInputTime = 0.08f;
    [SerializeField] float inputSensitivity = 3f;
    [SerializeField] float minMoveInput = 0.325f;

    [Header("Read Only")]
    [SerializeField] float speed = 0;
    [SerializeField] Vector3 lastNonZeroDir = Vector3.zero;
    [SerializeField] Vector3 vel = Vector3.zero;
    [SerializeField] Vector3 lastNonZeroVel = Vector3.zero;
    public Vector3 input = Vector3.zero;
    [SerializeField] float minInputTimeCounter = 0;
    [SerializeField] int movabilityDir = 0;
    [SerializeField] Vector3 usableInput = Vector3.zero;

    Vector3 dir = Vector3.zero;
    Transform currMovePoint = null;

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

        bool isZero = Mathf.Abs(minInputTimeCounter) < Mathf.Epsilon;

        if (isGoingToStop&&isZero)
            return;
        if (isGoingToMove&& minInputTimeCounter < 1)
            return;
        animator.SetFloat("Speed", speed);
    }
    void FixedUpdate()
    {
        minInputTimeCounter = minInputTimeCounter + Time.fixedDeltaTime / minInputTime * movabilityDir;
        minInputTimeCounter = Mathf.Clamp(minInputTimeCounter, 0, 1);

        usableInput = Vector3.MoveTowards(usableInput, input, inputSensitivity * Time.fixedDeltaTime);

        if (isGoingToStop && minInputTimeCounter < 0)
            return;
        if (isGoingToMove && minInputTimeCounter < 1)
            return;
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
        XZVel = vel;
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
            XZVel = Vector3.Slerp(cancelStartVel, Vector3.zero, f);
            speed = Mathf.Lerp(startCancelSpeed, 0, f);
            animator.SetFloat("Speed", speed);
        }).OnComplete(() => {
            XZVel = Vector3.zero;
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
    public void ReadFacingValues()
    {
        lastNonZeroDir = transform.forward;
        lastNonZeroVel = lastNonZeroDir * (stoppingSpeed + 0.05f);
    }
    public void UpdateInput(Vector3 newInput,Transform withRespectTo)
    {
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
        UpdateInput(newInput);
    }
    public void UpdateInput(Vector3 input)
    {
        currState = MovementState.ByInput;
        this.input = input;
        movabilityDir = 1;
        if (input == Vector3.zero)
        {
            movabilityDir = -1;
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
                if(isGoingToStop)
                    Cancel();
            }
        }
        speed = vel.magnitude;
        if(speed>dampingSpeed)
        {
            lastNonZeroVel = vel;
            vel = Vector3.ClampMagnitude(vel, maxSpeed);
        }
        XZVel = vel;
    }

    private void SetMoveDir()
    {
        dir = Vector3.zero;

        switch (currState)
        {
            case MovementState.ByInput:
                float ax = Mathf.Abs(usableInput.x);
                if (ax > 0)
                {
                    float usableX = ax < minMoveInput ? 0 : usableInput.x;
                    dir = dir + right * usableX;
                }
                float az = Mathf.Abs(usableInput.z);
                if (az > 0)
                {
                    float usableZ = az < minMoveInput ? 0 : usableInput.z;
                    dir = dir + fwd * usableZ;
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