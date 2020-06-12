using System;
using GW_Lib.Utility;
using UnityEngine;

public class BallReciever : DodgeballCharaAction, ICharaAction
{
    public Func<bool> CanDetectBall = () => true;
    public bool IsDetecting => isDetecting;

    [SerializeField] TriggerDelegator ballRecieveZone = null;

    [Header("Read Only")]
    [SerializeField] bool isBallIn = false;
    [SerializeField] bool isDetecting = false;

    public string actionName => "Recieve Ball";

    void OnEnable()
    {
        GetComponent<CharaHitPoints>().OnHPUpdated += OnHPUpdated;
        ballRecieveZone.onTriggerEnter.AddListener(OnEntered);
        ballRecieveZone.onTriggerExit.AddListener(OnExitted);
    }
    void OnDisable()
    {
        ballRecieveZone.onTriggerEnter.RemoveListener(OnEntered);
        ballRecieveZone.onTriggerExit.RemoveListener(OnExitted);
    }
    void Update()
    {
        if (!isDetecting)
            return;
        Bounds b = ballRecieveZone.GetCollider.bounds;
        Collider[] overlaps = Physics.OverlapBox(b.center, b.extents);
        foreach (var col in overlaps)
        {
            if (col.GetComponent<Dodgeball>())
                isBallIn = true;
        }
    }

    private void OnEntered(Collider other)
    {
        if (!CanDetectBall())
            return;

        Dodgeball ball = other.GetComponent<Dodgeball>();

        if (!ball)
            return;

        isBallIn = true;
    }
    private void OnExitted(Collider other)
    {
        if (!CanDetectBall())
            return;

        Dodgeball ball = other.GetComponent<Dodgeball>();

        if (!ball)
            return;

        isBallIn = false;
    }
    private void OnHPUpdated()
    {
        isDetecting = false;
    }

    public void StartReciptionAction()
    {
        isDetecting = true;
        Debug.Log("reception from team mates, has not been implemented yet, doing nothing");
    }
    public void Cancel()
    {

    }
}