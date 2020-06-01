using UnityEngine;

namespace GW_Lib.Utility
{
    public class AnimNumSetter : StateMachineBehaviour
    {
        enum SetPoint { OnEnter, OnExit }
        [SerializeField] SetPoint setPoint = SetPoint.OnEnter;
        [SerializeField] bool isInt = true;
        [SerializeField] bool useRand;

        [SerializeField] string variableKey = "";
        [SerializeField] float maxRand = 0;
        [SerializeField] float setVal = 0;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (setPoint == SetPoint.OnEnter)
            {
                SetVal(animator);
            }
        }
        public override void OnStateExit(UnityEngine.Animator animator, UnityEngine.AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (setPoint == SetPoint.OnExit)
            {
                SetVal(animator);
            }
        }
        private void SetVal(Animator animator)
        {
            if (isInt)
            {
                SetInt(animator);
            }
            else
            {
                SetFloat(animator);
            }
        }

        private void SetInt(Animator animator)
        {
            int v = Mathf.RoundToInt(setVal);
            if(useRand)
            {
                v = UnityEngine.Random.Range(0,Mathf.RoundToInt(maxRand));
            }
            animator.SetInteger(variableKey, v);
        }
        private void SetFloat(Animator animator)
        {
            float v = setVal;
            if(useRand)
            {
                v = UnityEngine.Random.Range(0,maxRand);
            }
            animator.SetFloat(variableKey, v);
        }
    }
}