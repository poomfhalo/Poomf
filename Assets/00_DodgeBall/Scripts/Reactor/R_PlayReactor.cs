using System.Collections;
using UnityEngine;

public class R_PlayReactor : Reaction
{
    [SerializeField] Reactor reactor = null;

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
        yield return StartCoroutine(reactor.ApplyReactions());
        isDone = true;
    }
}