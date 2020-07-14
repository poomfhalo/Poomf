using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DodgeballCharaCommand { MoveInput, Friendly, Enemy, BallAction, Dodge, FakeFire, Jump,
    BraceForBall,
    ReleaseFromBrace,
    PushBall,
    PathFollow
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
    public bool IsInField => isInField;

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
    [SerializeField] bool isInField = true;

    protected Rigidbody rb3d = null;
    protected Animator animator = null;
    protected Camera cam = null;

    protected Mover mover = null;
    public BallLauncher launcher = null;
    protected Dodger dodger = null;
    protected BallGrabber grabber = null;
    protected Jumper jumper = null;
    protected BallReciever reciever = null;
    protected CharaHitPoints hp = null;
    protected CharaFeet feet = null;
    protected PathFollower pathFollower = null;

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

        mover = GetComponent<Mover>();
        launcher = GetComponents<BallLauncher>().ToList().Find(a => a.enabled);
        dodger = GetComponent<Dodger>();
        grabber = GetComponent<BallGrabber>();
        jumper = GetComponent<Jumper>();
        reciever = GetComponent<BallReciever>();
        hp = GetComponent<CharaHitPoints>();
        feet = GetComponentInChildren<CharaFeet>();
        pathFollower = GetComponent<PathFollower>();

        SetTeam(team);

        selectionIndicator.SetOwner(this);
        selectionIndicator.SetFocus(null);

        hp.OnZeroHP += () => isInField = false;

        if(feet)
            feet.SetUp(this);
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
        if (IsDodging)
            return;
        if (IsBeingHurt)
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
    public void C_OnBallAction(InputActionPhase phase)
    {
        bool isDown = phase == InputActionPhase.Started || phase == InputActionPhase.Performed;
        GetComponent<BallReciever>().RecieveButtonInput(isDown);

        if (phase != InputActionPhase.Started)
            return;

        if (reciever.IsDetecting)
            return;//Recieving is handled in its own update
        
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
        if (IsJumping)
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
        if (reciever && !reciever.IsDetecting)
            reciever.EnableDetection();

        if (!hp.IsBeingHurt && !hp.IsWaitingForHit)
            hp.EnableHitDetection();
        OnCommandActivated?.Invoke(DodgeballCharaCommand.BraceForBall);
    }
    public void C_ReleaseFromBrace()
    {
        if(hp.IsWaitingForHit)
            hp.DisableHitDetection();
        if(reciever && reciever.IsDetecting)
            reciever.DisableDetection();

        OnCommandActivated?.Invoke(DodgeballCharaCommand.ReleaseFromBrace);
    }
    public void C_PushBall()
    {
        OnCommandActivated?.Invoke(DodgeballCharaCommand.PushBall);
    }
    /// <summary>
    /// Commands character to follow the path
    /// </summary>
    /// <param name="path">Path.</param>
    /// <param name="isLooping">If set to <c>true</c> is looping.</param>
    /// <param name="stopTimeAtPoint">Stop time at point, -1 means we do not stop at any point.</param>
    public void C_PathFollow(CharaPath path,bool isLooping,float stopTimeAtPoint)
    {
        pathFollower.StartFollowAction(path,isLooping,stopTimeAtPoint);
        OnCommandActivated?.Invoke(DodgeballCharaCommand.PathFollow);
    }
    #endregion

    public Vector3 PrepareForGame()
    {
        CharaSlot slot = GetComponent<CharaSlot>();
        CharaPath s = GameExtentions.GetPath(slot.GetID, PathType.GameStartPath, -1);

        transform.position = s.position;
        transform.rotation = s.rotation;

        gameObject.SetActive(true);

        SetTeam(slot.GetTeam);
        return s.position;
    }
}