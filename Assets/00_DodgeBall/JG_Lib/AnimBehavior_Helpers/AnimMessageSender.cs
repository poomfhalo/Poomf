using UnityEngine;

namespace GW_Lib.Utility
{
    public class AnimMessageSender : StateMachineBehaviour
    {
        enum SendPoint{OnEnter,OnExit}

        [SerializeField] SendPoint sendPoint = SendPoint.OnEnter;
        [SerializeField] string messageToSend = "Message";
        [SerializeField] SendMessageOptions messageOptions = SendMessageOptions.RequireReceiver;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(sendPoint == SendPoint.OnEnter)
            {
                animator.SendMessage(messageToSend,messageOptions);
            }
        }
        public override void OnStateExit(UnityEngine.Animator animator, UnityEngine.AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(sendPoint == SendPoint.OnExit)
            {
                animator.gameObject.SendMessage(messageToSend,messageOptions);
            }
        }
    }
}