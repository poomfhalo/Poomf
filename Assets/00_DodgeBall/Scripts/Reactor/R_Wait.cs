using System.Collections;
using UnityEngine;

public class R_Wait : Reaction
{
    [SerializeField] float waitTime = 1;
    bool isDone = false;

    protected override bool IsDone()
    {
        return isDone;
    }
    protected override void Initialize()
    {
        isDone = false;
    }
    protected override IEnumerator ReactionBehavior()
    {
        yield return new WaitForSeconds(waitTime);
        isDone = true;
    }
}