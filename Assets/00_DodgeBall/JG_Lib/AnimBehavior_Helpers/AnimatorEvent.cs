using UnityEngine;

public class AnimatorEvent : StateMachineBehaviour
{
    enum PlayOn {OnEnter,OnTime,OnExit}
    [SerializeField] string e = "A_EventName";
    [SerializeField] PlayOn playOn = PlayOn.OnEnter;
    [Range(0.0250f,1)]
    [SerializeField] float playPoint = 0.5f;
    [Header("Read Only")]
    [SerializeField] bool didPlayOnTime = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playOn == PlayOn.OnEnter)
        {
            animator.SendMessage(e,SendMessageOptions.RequireReceiver);
            return;
        }
        didPlayOnTime = false;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playOn == PlayOn.OnTime)
        {
            if(didPlayOnTime)
            {
                return;
            }
            if(stateInfo.normalizedTime>playPoint)
            {
                didPlayOnTime = true;
                animator.SendMessage(e,SendMessageOptions.RequireReceiver);
            }
            if(didPlayOnTime && Mathf.Approximately(stateInfo.normalizedTime,0))
            {
                didPlayOnTime = false;
            }
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playOn == PlayOn.OnExit)
        {
            animator.SendMessage(e,SendMessageOptions.RequireReceiver);
        }
    }
}
