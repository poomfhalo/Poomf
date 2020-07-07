using System.Collections;
using UnityEngine;

public class R_FadeScreen : Reaction
{
    public enum Command { FadeIn, FadeOut }
    [SerializeField] Command command = Command.FadeIn;
    [SerializeField] float dur = 1;

    bool isDone = false;
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
        if (!SceneFader.instance)
            yield break;

        switch (command)
        {
            case Command.FadeIn:
                SceneFader.instance.FadeIn(dur, null);
                break;
            case Command.FadeOut:
                SceneFader.instance.FadeOut(dur, null);
                break;
        }
        yield return new WaitForSeconds(dur + 0.01f);
        isDone = true;
    }
}