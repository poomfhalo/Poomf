using System.Collections;
using UnityEngine;

public class R_Debug : Reaction
{
    [SerializeField] Log.LogLevel logLevel = Log.LogLevel.Message;
    [SerializeField] string log = "";
    [SerializeField] Transform debugReference = null;

    protected override void Initialize()
    {

    }
    protected override bool IsDone()
    {
        return true;
    }
    protected override IEnumerator ReactionBehavior()
    {
        Log.LogByLevel(logLevel, log, debugReference);
        yield return 0;
    }
}
