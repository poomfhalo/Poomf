using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimatorEventMulti : StateMachineBehaviour
{
    public enum PlayOn { OnEnter, OnTime, OnExit }
    [System.Serializable]
    public class AnimEventData : IEqualityComparer<AnimEventData>
    {
        public string eName = "A_EventName";
        public PlayOn playOn = PlayOn.OnEnter;
        [Range(0.0250f, 1)]
        public float playPoint = 0.5f;
        [Header("Read Only")]
        public bool didPlayOnTime = false;

        public bool Equals(AnimEventData x, AnimEventData y) => x.IsSame(y);
        public int GetHashCode(AnimEventData obj) => base.GetHashCode();

        public bool IsSame(AnimEventData a)
        {
            if (eName == a.eName && playOn == a.playOn)
            {
                if(playOn == PlayOn.OnTime)
                {
                    float delta = Mathf.Abs(playPoint - a.playPoint);
                    if(Mathf.Abs(delta) < Mathf.Epsilon)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }
    [SerializeField] List<AnimEventData> animEvents = new List<AnimEventData>();

    int loopsCount = 0;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animEvents.Distinct(new AnimEventData());
        foreach (var e in animEvents)
        {
            if (e.playOn == PlayOn.OnEnter)
            {
                animator.SendMessage(e.eName, SendMessageOptions.RequireReceiver);
            }
            else if (e.playOn == PlayOn.OnTime)
            {
                e.didPlayOnTime = false;
            }
        }
        loopsCount = 0;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float f = stateInfo.normalizedTime;
        int newLoopsCount = Mathf.FloorToInt(f);
        Debug.Log("Curr " + loopsCount + " new " + newLoopsCount);
        foreach (var e in animEvents)
        {
            if (e.playOn == PlayOn.OnTime)
            {
                if (e.didPlayOnTime)
                {
                    return;
                }
                if (f > e.playPoint)
                {
                    e.didPlayOnTime = true;
                    animator.SendMessage(e.eName, SendMessageOptions.RequireReceiver);
                }
                if (e.didPlayOnTime && loopsCount != newLoopsCount)//it means, we entered a new loop for the animation
                {
                    e.didPlayOnTime = false;
                }
            }
        }
        loopsCount = newLoopsCount;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var e in animEvents)
        {
            if (e.playOn == PlayOn.OnExit)
            {
                animator.SendMessage(e.eName, SendMessageOptions.RequireReceiver);
            }
        }
    }
}