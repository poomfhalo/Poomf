using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class R_PlayTimeLine : Reaction
{
    [SerializeField] PlayableDirector director = null;
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
        director.Play();
        yield return new WaitForSeconds((float)director.duration);
        isDone = true;
    }
}