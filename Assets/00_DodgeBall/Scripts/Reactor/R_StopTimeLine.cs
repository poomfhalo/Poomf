using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class R_StopTimeLine : Reaction
{
    public enum Command { Play, Stop }
    [SerializeField] Command command = Command.Play;
    [SerializeField] PlayableDirector[] directors = null;
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
        foreach (var d in directors)
        {
            d.Stop();
        }
        yield return 0;
        isDone = true;
    }
}
