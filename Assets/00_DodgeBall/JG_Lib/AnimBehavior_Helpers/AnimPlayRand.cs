using UnityEngine;

namespace GW_Lib.Utility
{
    public class AnimPlayRand : StateMachineBehaviour
    {
        [SerializeField] string animKey = "RandAnim";
        [SerializeField] int animsCount = 2;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            int randAnim = Random.Range(0, animsCount);
            animator.SetFloat(animKey, randAnim);
        }
    }
}