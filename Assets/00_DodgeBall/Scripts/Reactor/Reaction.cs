using System.Collections;
using UnityEngine;

public enum WaitEndType { Time, TillDone, TillDonePlusTime }

public abstract class Reaction : MonoBehaviour
{
    [SerializeField] float startDelay = 0;
    [SerializeField] float duration = 0;
    [SerializeField] protected WaitEndType waitEndType= WaitEndType.Time;

    public IEnumerator React(Reactor caller)
    {
        yield return new WaitForSeconds(startDelay);
        Initialize();
        yield return 0;
        switch (waitEndType)
        {
            case WaitEndType.Time:
                yield return caller.StartCoroutine(TimedBehavior(caller));
                break;
            case WaitEndType.TillDone:
                yield return caller.StartCoroutine(ConditionalBehavior(caller));
                break;
            case WaitEndType.TillDonePlusTime:
                yield return caller.StartCoroutine(TimedConditionalBehavior(caller));
                break;
        }
    }

    protected abstract IEnumerator ReactionBehavior();
    protected abstract bool IsDone();
    protected abstract void Initialize();

    private IEnumerator TimedBehavior(Reactor caller)
    {
        caller.StartCoroutine(ReactionBehavior());
        yield return new WaitForSeconds(duration);
    }
    private IEnumerator ConditionalBehavior(Reactor caller)
    {
        yield return caller.StartCoroutine(ReactionBehavior());
        yield return new WaitUntil(IsDone);
    }
    private IEnumerator TimedConditionalBehavior(Reactor caller)
    {
        yield return caller.StartCoroutine(ReactionBehavior());
        yield return new WaitUntil(IsDone);
        yield return new WaitForSeconds(duration);
    }
}