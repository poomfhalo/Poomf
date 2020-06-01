using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public abstract class DodgeballCharacter : MonoBehaviour
{
    //State
    public bool IsEnabled => isEnabled;
    public bool HasBall => hasBall;
    public bool IsThrowing => launcher.IsThrowing;
    public bool IsMoving => mover.IsMoving;
    public bool IsDodging => dodger.IsDodging;

    //Body Parts
    public Transform BallGrabPoint => ballGrabPoint;
    public Transform ShootablePoint => shootablePoint;
    public Transform RecievablePoint => recievablePoint;
    public Vector3 position => rb3d.position;

    [Header("Body Parts")]
    [Tooltip("This transform represents where other characters will aim, when shooting at this character")]
    [SerializeField] Transform shootablePoint = null;
    [Tooltip("This transform represents where teammates will aim, when character sending the ball")]
    [SerializeField] Transform recievablePoint = null;
    [SerializeField] Transform ballGrabPoint = null;

    [Header("Core")]
    public string charaName = "";
    [SerializeField] TeamTag team = TeamTag.A;
    [SerializeField] bool isEnabled = false;
    [SerializeField] protected SelectionIndicator selectionIndicator = null;

    [Header("Read Only")]
    [SerializeField] bool hasBall = false;

    protected Rigidbody rb3d = null;
    protected Animator animator = null;
    protected Camera cam = null;

    protected Mover mover = null;
    protected BallLauncher launcher = null;
    protected Dodger dodger = null;

    protected virtual void Reset()
    {
        if (!GetComponent<CapsuleCollider>())
            gameObject.AddComponent<CapsuleCollider>();
        charaName = name;
        selectionIndicator = GetComponentInChildren<SelectionIndicator>();
    }
    protected virtual void Awake()
    {
        if(string.IsNullOrEmpty( charaName) || string.IsNullOrWhiteSpace(charaName))
        {
            charaName = name;
        }
        rb3d = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cam = Camera.main;

        launcher = GetComponent<BallLauncher>();
        mover = GetComponent<Mover>();
        dodger = GetComponent<Dodger>();


        if (selectionIndicator == null)
            selectionIndicator = GetComponentInChildren<SelectionIndicator>();
        if (selectionIndicator == null)
            selectionIndicator = ResFactory.Make<SelectionIndicator>("Selection Indicator", transform);

        SetTeam(team);

        selectionIndicator.SetOwner(this);
        selectionIndicator.SetFocus(null);

        if(launcher)
            launcher.onThrowPointReached += OnThrewBall;
    }

    public void SetTeam(TeamTag team)
    {
        this.team = team;
        TeamsManager.JoinTeam(team, this);
    }

    public virtual void InitCharacter()//To be used for multiplayer?
    {

    }

    protected void GrabBall()
    {
        if (HasBall)
            return;

        Dodgeball.GoTo(this, () => {
            hasBall = true;
            selectionIndicator.SetNewFocus(false);
        });
        animator.SetBool("HasBall", true);
    }
    protected void OnThrewBall()
    {
        hasBall = false;
        selectionIndicator.SetFocus(null);
    }
}