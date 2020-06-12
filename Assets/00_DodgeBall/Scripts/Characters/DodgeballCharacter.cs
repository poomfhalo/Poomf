using System;
using System.Linq;
using UnityEngine;

public enum DodgeballCharaCommand { MoveInput, Friendly, Enemy, BallAction, Dodge, FakeFire, Jump,
    BraceForBall
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class DodgeballCharacter : MonoBehaviour
{
    //Events
    public event Action<DodgeballCharaCommand> OnCommandActivated = null;

    //State
    public bool HasBall => grabber.HasBall;
    public bool IsThrowing => launcher.IsThrowing;
    public bool IsMoving => mover.IsMoving;
    public bool IsDodging => dodger.IsDodging;
    public bool IsBallInGrabZone => grabber.IsBallInGrabZone;
    public bool IsJumping => jumper.IsJumping;
    public bool IsBeingHurt => hp.IsBeingHurt;
    public bool IsWaitingForHit => hp.IsWaitingForHit;
    public string charaName => name;

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
    [SerializeField] TeamTag team = TeamTag.A;
    public SelectionIndicator selectionIndicator = null;

    protected Rigidbody rb3d = null;
    protected Animator animator = null;
    protected Camera cam = null;

    protected Mover mover = null;
    protected BallLauncher launcher = null;
    protected Dodger dodger = null;
    protected BallGrabber grabber = null;
    protected Jumper jumper = null;
    protected BallReciever reciever = null;
    protected CharaHitPoints hp = null;

    protected virtual void Reset()
    {
        if (!GetComponent<CapsuleCollider>())
            gameObject.AddComponent<CapsuleCollider>();
        selectionIndicator = GetComponentInChildren<SelectionIndicator>();
    }

    protected virtual void Awake()
    {
        rb3d = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cam = Camera.main;

        launcher = GetComponent<BallLauncher>();
        mover = GetComponent<Mover>();
        dodger = GetComponent<Dodger>();
        grabber = GetComponent<BallGrabber>();
        jumper = GetComponent<Jumper>();
        reciever = GetComponent<BallReciever>();
        hp = GetComponent<CharaHitPoints>();

        SetTeam(team);

        selectionIndicator.SetOwner(this);
        selectionIndicator.SetFocus(null);

        if (grabber)
            grabber.onBallInHands += OnBallInHands;
    }

    public void SetTeam(TeamTag team)
    {
        this.team = team;
        TeamsManager.JoinTeam(team, this);
    }

    protected void OnBallInHands()
    {
        selectionIndicator.SetNewFocus(false);
    }

    #region Input Commands
    public void C_MoveInput(Vector3 i)
    {
        GetComponents<DodgeballCharaAction>().ToList().ForEach(a => { a.RecieveInput(i); });

        if (IsJumping)
            return;
        if (IsThrowing)
            return;

        mover.StartMoveByInput(i, cam.transform);
        OnCommandActivated?.Invoke(DodgeballCharaCommand.MoveInput);
    }

    public void C_Friendly()
    {
        if (HasBall)
        {
            selectionIndicator.SetNewFocus(true);
            OnCommandActivated?.Invoke(DodgeballCharaCommand.Friendly);
        }
    }
    public void C_Enemy()
    {
        if (HasBall)
        {
            selectionIndicator.SetNewFocus(false);
            OnCommandActivated?.Invoke(DodgeballCharaCommand.Enemy);
        }
    }
    public void C_OnBallAction(bool hasStarted)
    {
        GetComponents<DodgeballCharaAction>().ToList().ForEach(a => a.RecieveButtonInput(hasStarted));
        if (!hasStarted)
            return;

        if (!HasBall && IsBallInGrabZone)
        {
            grabber.StartCatchAction();
            OnCommandActivated?.Invoke(DodgeballCharaCommand.BallAction);
        }

        if (HasBall && !IsThrowing)
        {
            launcher.StartThrowAction(selectionIndicator.ActiveSelection);
            OnCommandActivated?.Invoke(DodgeballCharaCommand.BallAction);
        }
    }
    public void C_Dodge()
    {
        if (HasBall)
            return;
        if (IsThrowing)
            return;
        if (IsDodging)
            return;

        dodger.StartDodgeAction();
        OnCommandActivated?.Invoke(DodgeballCharaCommand.Dodge);
    }
    public void C_FakeFire()
    {
        if (!HasBall)
            return;
        if (IsThrowing)
            return;
        launcher.StartFakeThrow(selectionIndicator.ActiveSelection);
        OnCommandActivated?.Invoke(DodgeballCharaCommand.FakeFire);
    }
    public void C_Jump()
    {
        if (HasBall)
            return;
        if (IsThrowing)
            return;
        if (IsDodging)
            return;
        if (IsJumping)
            return;
        jumper.StartJumpAction();
        OnCommandActivated?.Invoke(DodgeballCharaCommand.Jump);
    }
    #endregion

    #region Gameplay Commands
    public void C_BraceForContact()
    {
        reciever.StartReciptionAction();

        if (!hp.IsBeingHurt && !hp.IsWaitingForHit)
            hp.EnableHitDetection();
        OnCommandActivated?.Invoke(DodgeballCharaCommand.BraceForBall);
    }
    #endregion

}