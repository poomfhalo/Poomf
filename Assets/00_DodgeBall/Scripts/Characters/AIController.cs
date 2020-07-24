using GW_Lib;
using UnityEngine;

public class AIController : CharaController
{
    [SerializeField] float timeToReSendBall = 2;
    [SerializeField] bool autoRecieveball = true;

    [SerializeField] Transform moveTarget = null;
    [SerializeField] bool useWarp = false;


    [Header("Read Only")]
    [SerializeField] float lastDist = 0;
    [SerializeField] bool isLocked;

    Rigidbody rb3d = null;
    PC player = null;

    public override bool IsLocked { get => isLocked; protected set => isLocked = value; }

    void Start()
    {
        player = FindObjectOfType<PC>();
        rb3d = GetComponent<Rigidbody>();
        GetComponent<BallGrabber>().onBallInHands += () =>{
            this.InvokeDelayed(timeToReSendBall, () => {
                chara.SetFocus(player.chara);
                chara.C_OnBallAction(UnityEngine.InputSystem.InputActionPhase.Started);
            });
        };
    }
    void Update()
    {
        if(Dodgeball.instance.goTo.LastHolder != chara && chara.IsDetectingBallReciption && TeamsManager.AreFriendlies(chara,player.chara) && !chara.HasBall)
        {
            chara.C_OnBallAction(UnityEngine.InputSystem.InputActionPhase.Started);
            return;
        }
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!moveTarget)
            return;
        if (!IsLocked)
            return;

        Vector3 disp = moveTarget.position - rb3d.position;
        disp.y = 0;
        lastDist = disp.magnitude;
        if (useWarp)
        {
            transform.position = moveTarget.position;
        }
        else
        {
            chara.C_MoveInput(moveTarget.position);
        }
    }

    public override void Lock()
    {
        IsLocked = true;
    }
    public override void Unlock()
    {
        IsLocked = false;
    }
}