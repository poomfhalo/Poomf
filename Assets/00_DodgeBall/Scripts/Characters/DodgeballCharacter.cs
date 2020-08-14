using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DodgeballCharaCommand { MoveInput, Friendly, Enemy, BallAction, Dodge, FakeFire, Jump,
    PushBall,
    PathFollow,
    BraceForBallReciption,
    ReleaseFromBallReciptionBrace,

    EnableBallReciption,DisableBallReciption,
    EnableHitDetection,DisableHitDetection
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
    public bool IsInField => knockedOut.IsInField;
    public bool IsDetectingBallReciption => reciever.IsDetecting;

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
    [Header("Read Only")]
    [SerializeField] bool wasInitialized = false;

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
    protected CharaKnockoutPlayer knockedOut = null;

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
        knockedOut = GetComponent<CharaKnockoutPlayer>();

        SetTeam(team);

        selectionIndicator.SetOwner(this);
        selectionIndicator.SetFocus(null);

        if(feet)
            feet.SetUp(this);
        if (grabber)
            grabber.onBallInHands += OnBallInHands;

        TeamsManager.AddCharacter(this);
        wasInitialized = true;
    }
    void OnDestroy()
    {
        if(TeamsManager.instance)
            TeamsManager.GetTeam(this).Leave(this);
    }
    public void SetTeam(TeamTag team)
    {
        this.team = team;
        TeamsManager.JoinTeam(team, this);
    }
    public void SetFocus(DodgeballCharacter chara)
    {
        selectionIndicator.SetFocus(chara);
    }
    protected void OnBallInHands()
    {
        selectionIndicator.SetNewFocus(false);
    }

    #region Input Commands
    public void C_MoveInput(Vector3 i)
    {
        GetComponents<DodgeballCharaAction>().ToList().ForEach(a => { a.RecieveInput(i); });

        if (!wasInitialized)
            return;
        if (IsJumping)
            return;
        if (IsThrowing)
            return;
        if (IsDodging)
            return;
        if (IsBeingHurt)
            return;
        if(cam)
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
        if (!IsInField)
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
        if (IsThrowing)
            return;
        if (IsDodging)
            return;
        if (IsJumping)
            return;
        if (!IsInField)
            return;

        jumper.StartJumpAction();
        OnCommandActivated?.Invoke(DodgeballCharaCommand.Jump);
    }
    #endregion

    #region Gameplay Commands
    public void C_EnableHitDetection()
    {
        if (hp && !hp.IsWaitingForHit)
            hp.EnableHitDetection();

        OnCommandActivated?.Invoke(DodgeballCharaCommand.EnableHitDetection);
    }
    public void C_DisableHitDetection()
    {
        if (hp && hp.IsWaitingForHit)
            hp.DisableHitDetection();

        OnCommandActivated?.Invoke(DodgeballCharaCommand.DisableHitDetection);
    }
    public void C_EnableBallReciption()
    {
        if (reciever && !reciever.IsDetecting)
            reciever.EnableDetection();

        OnCommandActivated?.Invoke(DodgeballCharaCommand.EnableBallReciption);
    }
    public void C_DisableBallReciption()
    {
        if (reciever && reciever.IsDetecting)
            reciever.DisableDetection();

        OnCommandActivated?.Invoke(DodgeballCharaCommand.DisableBallReciption);
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
        Vector3 pos = s.position;
        Quaternion rot = s.rotation;
        if(!MatchState.Instance.IsFirstRound)
        {
            pos = s.finalPosition;
            rot = s.finalRotation;
        }
        transform.position = pos;
        transform.rotation = rot;

        gameObject.SetActive(true);

        SetTeam(slot.GetTeam);
        return s.position;
    }
}