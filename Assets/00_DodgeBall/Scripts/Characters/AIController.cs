using UnityEngine;

public class AIController : CharaController
{
    [SerializeField] Transform moveTarget = null;
    [SerializeField] bool useWarp = false;
    [Header("Read Only")]
    [SerializeField] float lastDist = 0;
    [SerializeField] bool isLocked;
    Rigidbody rb3d = null;

    public override bool IsLocked { get => isLocked; protected set => isLocked = value; }

    void Start()
    {
        rb3d = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (!moveTarget)
            return;
        if (!IsLocked)
            return;

        Vector3 disp = moveTarget.position - rb3d.position;
        disp.y = 0;
        lastDist = disp.magnitude;
        if(useWarp)
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