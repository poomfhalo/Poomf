using System;
using UnityEngine;

public class BallStallClock : MonoBehaviour
{
    [SerializeField] float stallingTime = 10;
    [SerializeField] float posCheckDist = 0.5f;
    [Tooltip("Shake Rate, means how many times, we shake, for the duration of stalling, " +
    	"0.1 means, every 10% of the waited time, we shake once")]
    [Range(0.05f, 0.9f)]
    [SerializeField] float shakeRate = 0.1f;
    [Tooltip("the strength multiplayer, as the duration goes on")]
    [SerializeField] float scaleShakeMulti = 2;
    [SerializeField] float posShakeMulti = 2;
    [Tooltip("If false, shakes, won't get stronger, as waiting is happening.")]
    [SerializeField] bool isConstantShake = false;

    [Header("Read Only")]
    [SerializeField] bool isStaling = false;
    [SerializeField] float stallingCounter = 0;
    [SerializeField] float shakeRateCounter = 0;

    public Func<bool> ExtAllowTeleport = () => true;

    Dodgeball ball = null;
    Vector3 startPos = new Vector3();

    Vector3 startPosXZ => new Vector3(startPos.x, 0, startPos.z);
    Vector3 currXZ => new Vector3(transform.position.x, 0, transform.position.z);
    BallShaker ballShaker = null;
    
    void Start()
    {
        ball = GetComponent<Dodgeball>();
        ballShaker = GetComponentInChildren<BallShaker>();

        ball.E_OnStateUpdated += OnStateUpdated;
        startPos = transform.position;
    }
    void Update()
    {
        if (!isStaling)
            return;

        stallingCounter += Time.deltaTime / stallingTime;
        shakeRateCounter += Time.deltaTime / stallingTime;
        if (shakeRateCounter > shakeRate)
        {
            shakeRateCounter = 0;
            if (IsAtPotentialTeleport())
            {
                float scaleSTR = scaleShakeMulti;
                float posSTR = posShakeMulti;
                if (!isConstantShake)
                {
                    scaleSTR = stallingCounter * scaleShakeMulti;
                    posSTR = stallingCounter * posShakeMulti;
                }
                ballShaker.ApplyShake(scaleSTR, posSTR);
            }
        }

        if (stallingCounter < 1)
            return;

        stallingCounter = 0;
        isStaling = false;

        if (IsAtPotentialTeleport() && ExtAllowTeleport())
        {
            ballShaker.CancelShake();
            transform.position = startPos;
        }
    }
    private bool IsAtPotentialTeleport()
    {
        float testDist = posCheckDist + GetComponent<SphereCollider>().radius;
        return Vector3.Distance(startPosXZ, currXZ) >= testDist && !GetComponent<Dodgeball>().IsHeld;
    }

    private void OnStateUpdated(Dodgeball.BallState state)
    {
        if (state == Dodgeball.BallState.StoppedOnGround)
        {
            isStaling = true;
            stallingCounter = 0;
        }
        else
        {
            ballShaker.CancelShake();
            isStaling = false;
            stallingCounter = 0;
        }
    }
}