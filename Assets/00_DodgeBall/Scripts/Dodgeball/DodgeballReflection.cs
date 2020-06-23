using System.Collections.Generic;
using System.Linq;
using GW_Lib;
using GW_Lib.Utility;
using UnityEngine;

public class DodgeballReflection : DodgeballAction
{
    [Header("Reflection Detection")]
    [SerializeField] bool autoActivate = false;
    [SerializeField] float castDist = 10;
    [Tooltip("Measured In Relation To Speed, based on the delta time, " +
        "1 means, if we're 1 frame away from contact, 5 means we start reacting when we're far by 5")]
    [SerializeField] int reflectionFramesPrediction = 3;
    [Tooltip("if true, we will use the longest delta time for the prediction duration, " +
    	"if false we will use the average")]
    [SerializeField] bool useSafestPrediction = false;

    [Header("Reflection Data")]
    [SerializeField] int maxTries = 60;
    [Tooltip("-1 means the ball must be exactly in opposite direction of hit character\n" +
    	"0 means perpindicular to hit character is the maximum direction of reflection\n" +
    	"1 means the ball have a chance to be reflected at all 360 angles\n" +
    	"everything else, is in between, depending on value")]
    [Range(-1, 1)]
    [SerializeField] float collisionDirThreshold = -0.75f;
    [SerializeField] MinMaxRange reflectionDist = new MinMaxRange(0.5f, 3, 0.8f, 2.5f);
    [SerializeField] MinMaxRange reflectionSpeedDivider = new MinMaxRange(1.1f, 6, 4.5f, 6f);


    [Header("Read Only")]
    [SerializeField] bool isReflecting = false;

    public override string actionName => "Reflection";
    public override DodgeballCommand Command => DodgeballCommand.Reflection;

    Vector3 travelVel = Vector3.zero;
    Vector3 lastPos = Vector3.zero;
    RaycastHit lastValidHit = new RaycastHit();

    bool didHit => lastValidHit.collider != null;
    Vector3 travelDir => travelVel.normalized;
    float expectedTravelDist
    {
        get
        {
            float usableFPS = avgFPSTime;
            if (useSafestPrediction)
                usableFPS = highestFPSTime;
            float val = travelSpeed* reflectionFramesPrediction *usableFPS;
            return val;
        }
    }
    float travelSpeed => travelVel.magnitude;

    float avgFPSTime = 0;
    int fpsCounter = 0;
    float fpsTimeCounter = 0;
    float highestFPSTime = 0;

    void Start()
    {
        if(autoActivate)
        {
            GetComponent<Dodgeball>().OnCommandActivated += (cmd) => {
                switch (cmd)
                {
                    case DodgeballCommand.LaunchTo:
                        Debug.Log("Started Reflection Action");
                        StartReflectionAction();
                        break;
                }
            };
        }
        avgFPSTime = Time.deltaTime;
        highestFPSTime = Time.deltaTime;
    }
    void Update()
    {
        UpdateExpectionDT();
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
        if (UpdateValidHit())
        {
            //Debug.Log(lastValidHit.collider.name);
            float dist = Vector3.Distance(lastValidHit.point, transform.position);
            if (dist <= expectedTravelDist)
            {
                didReflect = TryGetCollisionPoint(out Vector3 colPoint);
                Vector3 reflectionDir = (colPoint - rb3d.position).normalized;
                rb3d.velocity = reflectionDir * travelSpeed / reflectionSpeedDivider.GetValue();
                isReflecting = false;

                scheduler.StartAction(this);
                CharaHitPoints hp = lastValidHit.collider.GetComponent<CharaHitPoints>();
                hp.C_StartHitAction();
                ball.RunCommand(Command);
            }
        }
    }

    private bool TryGetCollisionPoint(out Vector3 collisionPoint)
    {
        int currTries = maxTries;
        Ray ray = new Ray(transform.position, Vector3.down);
        List<RaycastHit> hits = Physics.RaycastAll(ray, 30).ToList();
        RaycastHit floorHit = hits.Find(h => h.collider.GetComponent<Field>());

        do
        {
            currTries = currTries - 1;
            float a = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
            float x = Mathf.Cos(a);
            float z = Mathf.Sin(a);
            Vector3 dir = new Vector3(x, 0, z);
            float dot = Vector3.Dot(dir, travelDir);
            Vector3 rndPoint = floorHit.point + dir * reflectionDist.GetValue();

            if (dot < collisionDirThreshold)
            {
                collisionPoint = rndPoint;
                Extentions.LogSphere(rndPoint, Color.green, 0.35f);
                return true;

            }
        } while (currTries > 0);

        Vector3 flatTravelDir = travelDir;
        flatTravelDir.y = 0;
        flatTravelDir.Normalize();

        Vector3 backUpPos = floorHit.point - flatTravelDir*reflectionDist.GetValue();
        Extentions.LogSphere(backUpPos, Color.blue, 0.5f);
        collisionPoint = backUpPos;
        return false;
    }
    private bool UpdateValidHit()
    {
        lastValidHit = new RaycastHit();
        travelVel = (transform.position - lastPos) / Time.fixedDeltaTime;
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
    private void UpdateExpectionDT()
    {
        float currDt = Time.deltaTime;
        fpsTimeCounter = fpsTimeCounter + currDt;
        fpsCounter = fpsCounter + 1;
        if (fpsCounter >= reflectionFramesPrediction)
        {
            avgFPSTime = fpsTimeCounter / fpsCounter;
            fpsTimeCounter = 0;
            fpsCounter = 0;
            highestFPSTime = 0;
        }
        if (currDt > highestFPSTime)
        {
            highestFPSTime = currDt;
        }
    }
    public override void Cancel()
    {

    }
}