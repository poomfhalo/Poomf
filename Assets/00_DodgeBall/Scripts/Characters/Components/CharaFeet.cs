using System;
using UnityEngine;

public class CharaFeet : MonoBehaviour
{
    [SerializeField] float force = 5;
    [SerializeField] float timeBetweenPushes = 0.06f;

    [Header("Read Only")]
    public bool extCanPush = true;
    public Vector3 lastPushUsed = Vector3.zero;

    Vector3 lastPos = new Vector3();
    Rigidbody rb3d = null;
    Vector3 travelDir = Vector3.zero;
    float counter = 0;
    DodgeballCharacter owner = null;

    void Start()
    {
        rb3d = GetComponent<Rigidbody>();
        lastPos = transform.position;
    }
    void FixedUpdate()
    {
        travelDir = transform.position - lastPos;
        travelDir.Normalize();
        lastPos = transform.position;
        counter += Time.fixedDeltaTime/timeBetweenPushes;
        if (counter > 1)
            counter = 1;
    }
    private void OnTriggerStay(Collider col)
    {
        if (counter < 1)
            return;

        counter = 0;

        Vector3 forceV = travelDir * force;

        if (extCanPush)
        {
            lastPushUsed = forceV;
            owner.C_PushBall();
            ApplyPush(col.GetComponent<Dodgeball>(), forceV);
        }
    }

    public void SetUp(DodgeballCharacter owner)
    {
        this.owner = owner;
    }
    public void ApplyPush(Dodgeball ball,Vector3 force)
    {
        Rigidbody ballRB = ball.GetComponent<Rigidbody>();
        ballRB.AddForce(force, ForceMode.Impulse);
    }
}