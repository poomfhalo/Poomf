using System.Collections;
using System.Linq;
using UnityEngine;

public class R_FollowPath : Reaction
{
    [Tooltip("Each slot, has a single character, we will try to move that character")]
    [SerializeField] int slotID = 0;
    bool isDone = false;
    CharaController controller = null;
    SpawnPath path = null;
    void Reset()
    {
        waitEndType = WaitEndType.TillDone;
    }
    protected override void Initialize()
    {
        controller = FindObjectsOfType<CharaSlot>().ToList().Find(s => s.GetID == slotID).GetComponent<CharaController>();
        path = FindObjectsOfType<SpawnPath>().ToList().Find(s => s.CheckSlot(slotID));
        isDone = false;
    }
    protected override bool IsDone()
    {
        return isDone;  
    }
    protected override IEnumerator ReactionBehavior()
    {
        bool oldLockState = controller.IsLocked;

        controller.Lock();
        PathFollower pathFollower = controller.GetComponent<PathFollower>();

        pathFollower.StartFollowAction(path.transform);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !pathFollower.IsRunning);
        controller.SetLockTo(oldLockState);
        yield return 0;

        isDone = true;
    }
}
