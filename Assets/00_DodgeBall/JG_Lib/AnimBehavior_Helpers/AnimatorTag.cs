using System.Collections.Generic;
using UnityEngine;

public class AnimatorTag : StateMachineBehaviour
{
    public bool isRunning { private set; get; } = false;

    [Tooltip("Aviliable Tags: LockMovement,")]
    [SerializeField] List<string> tags = new List<string>() { "LockMovement" };

    AnimatorStateInfo state;

    public bool HasTag(string s)
    {
        return tags.Contains(s);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isRunning = true;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isRunning = false;
    }
}