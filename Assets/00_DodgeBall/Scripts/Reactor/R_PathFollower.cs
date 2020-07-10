using System.Collections;
using System.Collections.Generic;
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
        List<CharaController> controllers = new List<CharaController>();
        List<SpawnPath> paths = new List<SpawnPath>();
        List<bool> oldStates = new List<bool>();
        foreach (var slotToMove in slotsToMove)
        {
            CharaController controller = DodgeballGameManager.GetCharaOfSlot(slotToMove);
            if (controller == null)
                continue;

            List<SpawnPath> pathsObjs = DodgeballGameManager.GetPathsOfSlot(slotToMove);
            SpawnPath path = pathsObjs[0];

            PathFollower pathFollower = controller.GetComponent<PathFollower>();

            if (pathFollower.extCanPlayAction)
            {
                controllers.Add(controller);
                paths.Add(path);
                oldStates.Add(controller.IsLocked);
            }
        }

        for (int i = 0; i < controllers.Count;i++)
        {
            CharaController controller = controllers[i];
            SpawnPath path = paths[i];
            //no longer needed, for reason below.
            //controller.Lock();
            DodgeballCharacter chara = controller.GetComponent<DodgeballCharacter>();
            chara.C_PathFollow(path.transform, false);
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

        //No Longer Needed, as we sync, whether the PathFollower, controls locking input or not, so, 
        //Pathfollower, will not control Locking, from the reaction, as R_SetCharaLock is the one that is responsible
        //for locking the characters.
        //for(int i =0;i<controllers.Count;i++)
        //{
        //    CharaController controller = controllers[i];
        //    controller.SetLockTo(oldStates[i]);
        //}

        yield return 0;

        isDone = true;
    }
}