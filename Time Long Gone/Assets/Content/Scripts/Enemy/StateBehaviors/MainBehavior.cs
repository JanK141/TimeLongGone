using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Enemy.AI_Conditions.Templates;
using Content.Scripts.Player;
using UnityEngine;

public class MainBehavior : StateMachineBehaviour
{
    [SerializeField] [Min(0)] private float MinWaitTime;
    [SerializeField] [Min(0)] private float MaxWaitTime;
    [SerializeField] private List<AICondition> chasePlayerConditions;

    private float timeToTrigger;
    private int chaseTest = 0;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if(param.type.Equals(AnimatorControllerParameterType.Trigger))
                animator.ResetTrigger(param.name);
        }

        timeToTrigger = Time.time + (Random.Range(MinWaitTime, MaxWaitTime));
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        chaseTest = 0;
        foreach (AICondition condition in chasePlayerConditions)
            if (condition.Check(animator.gameObject, PlayerScript.Instance.gameObject)) chaseTest++;
        if(chaseTest>=chasePlayerConditions.Count) animator.SetTrigger("Walk");
        else if(Time.time >= timeToTrigger) animator.SetTrigger("Attack");
    }

}
