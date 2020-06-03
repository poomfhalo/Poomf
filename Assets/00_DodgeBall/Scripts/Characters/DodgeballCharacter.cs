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
    public bool IsBallInGrabZone => catcher.IsBallInGrabZone;
    public bool IsJumping => jumper.IsJumping;
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
    protected BallCatcher catcher = null;
    protected Jumper jumper = null;

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
        catcher = GetComponent<BallCatcher>();
        jumper = GetComponent<Jumper>();

        if (selectionIndicator == null)
            selectionIndicator = GetComponentInChildren<SelectionIndicator>();
        if (selectionIndicator == null)
            selectionIndicator = ResFactory.Make<SelectionIndicator>("Selection Indicator", transform);

        SetTeam(team);

        selectionIndicator.SetOwner(this);
        selectionIndicator.SetFocus(null);

        if(launcher)
            launcher.onThrowPointReached += OnThrewBall;

        if (catcher)
            catcher.onBallInHands += OnBallInHands;
    }

    public void SetTeam(TeamTag team)
    {
        this.team = team;
        TeamsManager.JoinTeam(team, this);
    }

    protected void OnThrewBall()
    {
        hasBall = false;
        selectionIndicator.SetFocus(null);
    }
    private void OnBallInHands()
    {
        hasBall = true;
        selectionIndicator.SetNewFocus(false);
    }


    #region Commands
    protected void C_MoveInput(Vector3 i)
    {
        if (jumper.IsJumping)
        {
            jumper.UpdateInput(i);
            return;
        }
        mover.StartMoveByInput(i, cam.transform);
    }
    protected void C_Catch()
    {
        if (HasBall)
            return;
        if (!IsBallInGrabZone)
            return;

        catcher.StartCatchAction();
    }
    protected void C_Friendly()
    {
        if (HasBall)
        {
            selectionIndicator.SetNewFocus(true);
        }
    }
    protected void C_Enemy()
    {
        if (HasBall)
        {
            selectionIndicator.SetNewFocus(false);
        }
    }
    protected void C_Fire()
    {
        if (IsThrowing)
            return;
        if (!HasBall)
            return;
        launcher.UpdateInput(mover.input);
        launcher.StartThrowAction(selectionIndicator.ActiveSelection);
    }
    protected void C_Dodge()
    {
        if (HasBall)
            return;
        if (IsThrowing)
            return;
        if (IsDodging)
            return;

        dodger.StartDodgeAction();
    }
    protected void C_FakeFire()
    {
        if (!HasBall)
            return;
        if (IsThrowing)
            return;
        launcher.UpdateInput(mover.input);
        launcher.StartFakeThrow(selectionIndicator.ActiveSelection);
    }
    protected void C_Jump()
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

    }
    #endregion


}