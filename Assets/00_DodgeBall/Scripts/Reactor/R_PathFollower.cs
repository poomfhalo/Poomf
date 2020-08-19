using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_PathFollower : Reaction
{
    [Serializable]
    private class CharaPathFollowData
    {
        [Tooltip("Each slot, has a single character, we will try to move that character")]
        public int slotToMove;
        public PathType pathTag;
        public int pathIndex;
    }

    [Tooltip("Each slot, has a single character, we will try to move that character")]
    [SerializeField] CharaPathFollowData[] pathsDatas = new CharaPathFollowData[0];
    [SerializeField] float stopTimeAtPoint = -1;
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
        List<CharaPathFollowData> paths = new List<CharaPathFollowData>();
        List<bool> oldStates = new List<bool>();
        foreach (var pathData in pathsDatas)
        {
            CharaController controller = GameExtentions.GetCharaOfID(pathData.slotToMove);
            if (controller == null)
                continue;

            PathFollower pathFollower = controller.GetComponent<PathFollower>();

            if (pathFollower.extCanPlayAction)
            {
                controllers.Add(controller);
                paths.Add(pathData);
                oldStates.Add(controller.IsLocked);
            }
        }

        for (int i = 0; i < controllers.Count;i++)
        {
            CharaController controller = controllers[i];
            //no longer needed, for reason below.
            //controller.Lock();
            DodgeballCharacter chara = controller.GetComponent<DodgeballCharacter>();
            CharaPath path = GameExtentions.GetPath(paths[i].slotToMove, paths[i].pathTag, paths[i].pathIndex);
            chara.C_PathFollow(path,false, stopTimeAtPoint);
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