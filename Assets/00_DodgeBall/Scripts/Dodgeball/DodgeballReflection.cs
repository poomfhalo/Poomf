using System.Collections.Generic;
using System.Linq;
using GW_Lib;
using UnityEngine;

public class DodgeballReflection : DodgeballAction
{
    [Header("Reflection Detection")]
    [SerializeField] float castDist = 10;
    [SerializeField] SphereCollider contactZone = null;
    [SerializeField] float reflectionDist = 0.65f;
    [SerializeField] bool autoActivate = false;

    [Header("Reflection Data")]
    [SerializeField] int maxTries = 60;

    [Header("Read Only")]
    [SerializeField] bool isReflecting = false;

    public override string actionName => "Reflection";
    public override DodgeballCommand Command => DodgeballCommand.Reflection;

    Vector3 travelDir = Vector3.zero;
    Vector3 lastPos = Vector3.zero;
    RaycastHit lastValidHit = new RaycastHit();
    bool didHit => lastValidHit.collider != null;

    void Start()
    {
        if(autoActivate)
        {
            GetComponent<Dodgeball>().OnCommandActivated += (cmd) => {
                switch (cmd)
                {
                    case DodgeballCommand.LaunchTo:
                        StartReflectionAction();
                        break;
                }
            };
        }
    }
    void Update()
    {
        if (GetComponent<Dodgeball>().IsOnGround)
            isReflecting = false;
        if (!isReflecting)
            return;

        if (isReflecting)
            TryReflect();

        if (didHit)
        {
            Debug.DrawRay(lastValidHit.point, lastValidHit.normal * castDist, Color.yellow);
            Debug.DrawRay(lastValidHit.point, GetReflected(), Color.red);
        }
        lastPos = transform.position;
    }

    public void StartReflectionAction()
    {
        lastPos = transform.position;
        isReflecting = true;
        TryReflect();
    }

    private void TryReflect()
    {
        bool didReflect = false;
        if(UpdateValidHit())
        {
            float dist = Vector3.Distance(lastValidHit.point, transform.position);
            if(dist<=reflectionDist)
            {
                int currTries = maxTries;
                do
                {
                    currTries = currTries - 1;
                    Ray ray = new Ray(transform.position, Vector3.down);
                    List<RaycastHit> hits = Physics.RaycastAll(ray, 30).ToList();
                    RaycastHit floorHit = hits.Find(h => h.collider.GetComponent<Field>());
                    float a = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
                    float x = Mathf.Cos(a);
                    float z = Mathf.Sin(a);
                    Vector3 dir = new Vector3(x, 0, z);
                } while (currTries>0);
            }
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
    private Vector3 GetReflected()
    {
        if (!didHit)
            return Vector3.zero;
        return Vector3.Reflect(travelDir, lastValidHit.normal);
    }
    public override void Cancel()
    {

    }
}