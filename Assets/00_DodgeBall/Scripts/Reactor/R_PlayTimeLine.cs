using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class R_PlayTimeLine : Reaction
{
    public enum Command { Play, Stop }
    [SerializeField] Command command = Command.Play;
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
        switch (command)
        {
            case Command.Play:
                director.Play();
                yield return new WaitForSeconds((float)director.duration);
                break;
            case Command.Stop:
                director.Stop();
                break;
        }
        isDone = true;
    }
}