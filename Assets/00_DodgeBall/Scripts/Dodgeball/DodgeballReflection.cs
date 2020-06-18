using UnityEngine;

public class DodgeballReflection : DodgeballAction
{
    [Header("Reflection Detection")]
    [SerializeField] float castDist = 0.2f;
    [SerializeField] SphereCollider contactZone = null;
    [Header("Read Only")]
    [SerializeField] bool isReflecting = false;
    [SerializeField] bool gotValidHit = false;

    public override string actionName => "Reflection";
    public override DodgeballCommand Command => DodgeballCommand.Reflection;

    Vector3 travelDir = Vector3.zero;
    Vector3 lastPos = Vector3.zero;
    RaycastHit lastValidHit = new RaycastHit();
    bool didHit => lastValidHit.collider != null;

    public void StartReflectionAction()
    {
        isReflecting = true;
        lastPos = transform.position;
        TryReflect();
    }

    void Update()
    {
        UpdateValidHit();

        if (isReflecting)
        {
            TryReflect();
        }
        if (didHit)
        {
            Debug.DrawRay(lastValidHit.point, lastValidHit.normal * castDist, Color.yellow);
            Vector3 reflected = Vector3.Reflect(travelDir, lastValidHit.normal);
            Debug.DrawRay(lastValidHit.point, reflected, Color.red);
        }
        lastPos = transform.position;
    }

    private void TryReflect()
    {
        bool didReflect = false;
        if(UpdateValidHit())
        {

        }
        if (didReflect)
        {
            ball.RunCommand(Command);
        }
    }

    private bool UpdateValidHit()
    {
        lastValidHit = new RaycastHit();
        travelDir = transform.position - lastPos;
        travelDir.Normalize();
        Ray ray = new Ray(transform.position, travelDir);
        RaycastHit[] hits = Physics.RaycastAll(ray,castDist);
        Debug.DrawRay(transform.position, travelDir * castDist, Color.blue);
        foreach (var h in hits)
        {
            DodgeballCharacter chara = h.collider.GetComponent<DodgeballCharacter>();
            if (chara)
            {
                lastValidHit = h;
                return true;
            }
        }
        return false;
    }

    public override void Cancel()
    {

    }
}