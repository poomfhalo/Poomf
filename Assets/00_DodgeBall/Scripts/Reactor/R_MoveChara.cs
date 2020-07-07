﻿using System.Collections;
using UnityEngine;

public class R_MoveChara : Reaction
{
    [SerializeField] CharaController controller = null;
    [SerializeField] Transform target = null;
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
        Mover mover = controller.GetComponent<Mover>();
        Mover.MovementType oldMoveType = mover.movementMode;
        mover.movementMode = Mover.MovementType.ToPoint;

        controller.chara.C_MoveInput(target.position);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !mover.IsMoving);

        isDone = true;
        mover.movementMode = oldMoveType;
    }
}