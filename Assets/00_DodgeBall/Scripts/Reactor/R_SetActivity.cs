using System.Collections;
using UnityEngine;

public class R_SetActivity : Reaction
{
    [SerializeField] GameObject[] objs = new GameObject[0];
    [SerializeField] bool activity = false;

    protected override void Initialize()
    {

    }
    protected override bool IsDone()
    {
        return true;
    }
    protected override IEnumerator ReactionBehavior()
    {
        foreach (var g in objs)
        {
            g.SetActive(activity);
        }
        yield return 0;
    }
}
