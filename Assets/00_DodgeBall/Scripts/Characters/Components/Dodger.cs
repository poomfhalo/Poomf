using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodger : DodgeballCharaAction, ICharaAction
{
    public bool IsDodging => isDodging;
    public string actionName => "Dodge Action";

    [Tooltip("Number of animations, of Dodges in the Dodge sub state machine in the animator")]
    [SerializeField] int dodgesCount = 2;
    [SerializeField] bool playRndDodge = true;
    [Tooltip("If playRndDodge is true, this will be randomly set, otherwise, we will dodge using this animation only")]
    [SerializeField] int dodgeToPlay = 0;

    [Header("Read Only")]
    [SerializeField] bool isDodging = false;
    Animator animator = null;
    ActionsScheduler scheduler = null;

    void Start()
    {
        animator = GetComponent<Animator>();
        scheduler = GetComponent<ActionsScheduler>();
    }

    public void StartDodgeAction()
    {
        animator.applyRootMotion = true;
        if(playRndDodge)
        {
            dodgeToPlay = UnityEngine.Random.Range(0, dodgesCount);
        }
        animator.SetInteger("DodgeType", dodgeToPlay);
        animator.SetTrigger("Dodge");
        scheduler.StartAction(this, false);
        isDodging = true;
    }

    public void Cancel()
    {

    }

    public void A_OnDodgeCompleted()
    {
        animator.applyRootMotion = false;
        isDodging = false;
    }

}