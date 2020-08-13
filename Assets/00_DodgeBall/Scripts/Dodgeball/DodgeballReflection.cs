using System;
using System.Collections.Generic;
using System.Linq;
using GW_Lib;
using GW_Lib.Utility;
using UnityEngine;

public class DodgeballReflection : DodgeballAction
{
    public enum FPSType { Min, Avg, Max }
    public event Action onReflected = null;
    [Header("Reflection Detection")]
    [SerializeField] bool autoActivate = false;
    [SerializeField] float castDist = 10;
    [Tooltip("Measured In Relation To Speed, based on the delta time, " +
        "1 means, if we're 1 frame away from contact, 5 means we start reacting when we're far by 5")]
    [SerializeField] int reflectionFramesPrediction = 3;
    [Tooltip("if true, we will use the longest delta time for the prediction duration, " +
        "if false we will use the average")]
    [SerializeField] FPSType fpsType = FPSType.Min;
    [Tooltip("How many times the script tries to find a proper collision point before using the backup point\n" +
        "back up point is usually in direct opposite position")]
    [SerializeField] int maxTries = 60;

    [Header("Reflection Data")]
    [Tooltip("-1 means the ball must be exactly in opposite direction of hit character\n" +
    	"0 means perpindicular to hit character is the maximum direction of reflection\n" +
    	"1 means the ball have a chance to be reflected at all 360 angles\n" +
    	"everything else, is in between, depending on value")]
    [Range(-1, 1)]
    [SerializeField] float collisionDirThreshold = -0.75f;
    [Tooltip("How far the reflection point is")]
    [SerializeField] MinMaxRange reflectionDist = new MinMaxRange(0.5f, 3, 0.8f, 2.5f);
    [Tooltip("the number that the speed of the ball gets divided by")]
    [SerializeField] MinMaxRange reflectionSpeedDivider = new MinMaxRange(1.1f, 6, 4.5f, 6f);
    [Tooltip("The Reflection, will do tis best, so that, the ball, never reaches closer than this distance to the player.")]
    [SerializeField] float minReflectionDist = 1.35f;

    [Header("Read Only")]
    public bool extReflectionTest = true;
    public Vector3 lastReflectionVel = new Vector3();
    public Vector3 lastReflectionTarget = new Vector3();
    public Vector3 lastReflectionStartPoint = new Vector3();
    public GameObject lastContact = null;
    public bool MakesLogSpheres = true;
    public List<GameObject> loggedSpheres = new List<GameObject>();

    public override string actionName => "Reflection";
    public override DodgeballCommand Command => DodgeballCommand.Reflection;

    Vector3 travelVel = Vector3.zero;
    Vector3 lastPos = Vector3.zero;
    RaycastHit lastValidHit = new RaycastHit();

    bool didHit => lastValidHit.collider != null;
    Vector3 travelDir => travelVel.normalized;
    float lowestFPS = 0;
    float expectedTravelDist
    {
        get
        {
            float usableFPS = avgFPSTime;
            switch (fpsType)
            {
                case FPSType.Max:
                    usableFPS = highestFPSTime;
                    break;
                case FPSType.Min:
                    usableFPS = lowestFPS;
                    break;
            }
            float val = travelSpeed* reflectionFramesPrediction *usableFPS;

            float highest = Mathf.Max(minReflectionDist, lowestTravelDist);
            if (val < highest)
                val = highest;

            return val;
        }
    }
    float travelSpeed => travelVel.magnitude;

    float avgFPSTime = 0;
    int fpsCounter = 0;
    float fpsTimeCounter = 0;
    float highestFPSTime = 0;
    [SerializeField] float lowestTravelDist = float.MaxValue;

    void Start()
    {
        if(autoActivate && extReflectionTest)
        {
            GetComponent<Dodgeball>().E_OnCommandActivated += (cmd) => {
                switch (cmd)
                {
                    case DodgeballCommand.LaunchTo:
                        Debug.Log("Started Reflection Action");
                        StartReflectionDetection();
                        break;
                    case DodgeballCommand.HitGround:
                        Cancel();
                        break;
                    case DodgeballCommand.GoToChara:
                        Cancel();
                        break;
                }
            };
        }
        avgFPSTime = Time.deltaTime;
        highestFPSTime = Time.deltaTime;
        lowestFPS = Time.deltaTime;
    }
    void Update()
    {
        UpdateExpectionDT();

        if (!IsRunning)
            return;
        if (IsRunning)
            C_Reflect();

        if (didHit)
        {
            Debug.DrawRay(lastValidHit.point, lastValidHit.normal * castDist, Color.yellow);
        }
        lastPos = transform.position;
    }

    private void StartReflectionDetection()
    {
        lowestTravelDist = float.MaxValue;
        lastPos = transform.position;
        isRunning = true;
        C_Reflect();
    }

    public void Reflect(Vector3 vel,Vector3 startPoint, Vector3 endPoint,GameObject contactWith)
    {
        scheduler.StartAction(this);
        transform.position = startPoint;
        rb3d.velocity = vel;

        if(MakesLogSpheres)
            loggedSpheres.Add(Extentions.LogSphere(endPoint, Color.green, 0.35f));

        CharaHitPoints hp = contactWith.GetComponent<CharaHitPoints>();
        hp.C_StartHitAction(1, GetComponent<Dodgeball>().lastThrower);
        onReflected?.Invoke();
    }

    private void C_Reflect()
    {
        if (!IsRunning)
            return;
        if (!extReflectionTest)
            return;

        if (UpdateValidHit())
        {
            BallReciever currReciever = lastValidHit.collider.GetComponent<BallReciever>();
            if (currReciever.CanRecieveBallNow || currReciever.justCaughtBall)
                return;

            float dist = Vector3.Distance(lastValidHit.point, transform.position);
            if (dist <= expectedTravelDist)
            {
                this.SetKinematic(this, false);
                if (MakesLogSpheres)
                    loggedSpheres.Add(Extentions.LogSphere(lastValidHit.point, Color.magenta, 0.35f));
                Log.LogL0("Reflected With This Data Dist " + dist + " :: Expected Dist :: " + expectedTravelDist + " :: FPS " + avgFPSTime + " With Speed " + travelSpeed);
                SetReflectionData();

                ball.RunCommand(Command);
                Reflect(lastReflectionVel, lastReflectionStartPoint, lastReflectionTarget,lastContact);

                Cancel();
            }
        }
    }
    private void SetReflectionData()
    {
        TryGetCollisionPoint(out Vector3 colPoint);
        Vector3 reflectionDir = (colPoint - rb3d.position).normalized;
        Vector3 vel = reflectionDir * travelSpeed / reflectionSpeedDivider.GetValue();

        lastReflectionVel = vel;
        lastReflectionTarget = colPoint;
        lastReflectionStartPoint = transform.position;
        lastContact = lastValidHit.collider.gameObject;
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
                return true;

            }
        } while (currTries > 0);

        Vector3 flatTravelDir = travelDir;
        flatTravelDir.y = 0;
        flatTravelDir.Normalize();

        Vector3 backUpPos = floorHit.point - flatTravelDir*reflectionDist.GetValue();

        if(MakesLogSpheres)
            loggedSpheres.Add(Extentions.LogSphere(backUpPos, Color.blue, 0.5f));

        collisionPoint = backUpPos;
        return false;
    }
    private bool UpdateValidHit()
    {
        lastValidHit = new RaycastHit();

        travelVel = (transform.position - lastPos) / Time.deltaTime;
        Ray ray = new Ray(transform.position, travelDir);
        RaycastHit[] hits = Physics.RaycastAll(ray,castDist);
        Debug.DrawRay(transform.position, travelDir * castDist, Color.blue);
        foreach (var h in hits)
        {
            DodgeballCharacter chara = h.collider.GetComponent<DodgeballCharacter>();
            if (chara)
            {
                lastValidHit = h;
                float dist = (transform.position - lastPos).magnitude;
                if (dist <= lowestTravelDist)
                {
                    lowestTravelDist = dist;
                }

                return true;
            }
        }
        return false;
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
            highestFPSTime = Time.deltaTime;
            lowestFPS = Time.deltaTime;
        }

        if (currDt > highestFPSTime)
            highestFPSTime = currDt;
        if (currDt < lowestFPS)
            lowestFPS = currDt;
    }
    public override void Cancel()
    {
        isRunning = false;
    }
}