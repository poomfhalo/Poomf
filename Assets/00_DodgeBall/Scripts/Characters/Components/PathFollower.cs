using UnityEngine;

public class PathFollower : DodgeballCharaAction, ICharaAction
{
    public bool IsRunning = false;
    public string actionName => "Follow Path";

    [Tooltip("Detecting when we're too close to the next point, noting that it shoudl be higher thna stopping dist of Mover")]
    [SerializeField] float satisfactionRadious = 0.5f;
    [Tooltip("If 0 or less then we will not stop at all, when we are at a point, we will move the next right away")]
    [SerializeField] float stopTimeAtPoint = -1;

    [Header("Read Only")]
    [SerializeField] Transform activePathHead = null;
    [SerializeField] int activePoint = 0;
    [SerializeField] bool changingPoint = false;
    [SerializeField] float changingPointCounter = 0;
    DodgeballCharacter chara = null;
    Mover mover = null;
    void Start()
    {
        chara = GetComponent<DodgeballCharacter>();
        mover = GetComponent<Mover>();
    }
    void Update()
    {
        if (activePathHead == null)
            return;
        if (activePathHead.childCount == 0)
            return;
        if (activePoint >= activePathHead.childCount && IsRunning)
        {
            Cancel();
            return;
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = activePathHead.GetChild(activePoint).position;
        startPos.y = endPos.y = 0;
        float testDist = Vector3.Distance(startPos, endPos);
        if (testDist <= satisfactionRadious && !mover.IsMoving)
        {
            activePoint = activePoint + 1;
        }

        bool isLastPoint = activePoint >= activePathHead.childCount - 1;
        if (!isLastPoint && !changingPoint)
        {
            endPos = activePathHead.GetChild(activePoint + 1).position;
            startPos.y = endPos.y = 0;
            testDist = Vector3.Distance(startPos, endPos);
            if (testDist <= satisfactionRadious && stopTimeAtPoint <= 0)
            {
                activePoint = activePoint + 1;
                changingPoint = true;
                return;
            }
        }
    
        if (activePoint >= activePathHead.childCount)
        {
            Cancel();
            return;
        }
        if (changingPoint)
        {
            changingPointCounter = changingPointCounter + Time.deltaTime / stopTimeAtPoint;
            if(changingPointCounter>=1)
            {
                changingPoint = false;
                changingPointCounter = 0;
            }
            return;
        }
        Vector3 activePos = activePathHead.GetChild(activePoint).position;
        activePos.y = 0;
        chara.C_MoveInput(activePos);
    }

    public void StartFollowAction(Transform pathHead)
    {
        GetComponent<Mover>().ReadFacingValues();
        activePathHead = pathHead;
        GetComponent<CharaController>().Lock();
        IsRunning = true;
        GetComponent<Mover>().workAsAction = false;
    }
    public void Cancel()
    {
        IsRunning = false;
        activePathHead = null;
        changingPoint = false;
        activePoint = 0;
        GetComponent<CharaController>().Unlock();
        GetComponent<Mover>().workAsAction = true;
    }
}