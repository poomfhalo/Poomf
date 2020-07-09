using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class R_PathFollower : Reaction
{
    [Tooltip("Each slot, has a single character, we will try to move that character")]
    [SerializeField] int[] slotsToMove = new int[0];
    bool isDone = false;

    void Reset()
    {
        waitEndType = WaitEndType.TillDone;
    }
    protected override void Initialize()
    {
        isDone = false;
    }
    protected override bool IsDone()
    {
        return isDone;  
    }
    protected override IEnumerator ReactionBehavior()
    {
        List<bool> oldStates = new List<bool>();
        List<CharaController> controllers = new List<CharaController>();
        List<SpawnPath> paths = new List<SpawnPath>();

        foreach (var slotToMove in slotsToMove)
        {
            CharaController controller = DodgeballGameManager.GetCharaOfSlot(slotToMove);
            List<SpawnPath> pathsObjs = DodgeballGameManager.GetPathsOfSlot(slotToMove);
            SpawnPath path = pathsObjs[0];

            PathFollower pathFollower = controller.GetComponent<PathFollower>();

            if (pathFollower.extCanPlayAction)
            {
                controllers.Add(controller);
                paths.Add(path);
                bool oldLockState = controller.IsLocked;
                oldStates.Add(oldLockState);
            }
        }

        for (int i = 0; i < controllers.Count;i++)
        {
            CharaController controller = controllers[i];
            SpawnPath path = paths[i];
            controller.Lock();
            DodgeballCharacter chara = controller.GetComponent<DodgeballCharacter>();
            chara.C_PathFollow(path.transform);
        }

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => { 
            for(int i = 0;i<controllers.Count;i++)
            {
                PathFollower pathFollower = controllers[i].GetComponent<PathFollower>();
                if (pathFollower.IsRunning)
                    return false;
            }
            return true;
        });
        yield return new WaitForSeconds(0.3f);

        for (int i =0;i<controllers.Count;i++)
        {
            controllers[i].SetLockTo(oldStates[i]);
        }

        yield return 0;

        isDone = true;
    }
}
