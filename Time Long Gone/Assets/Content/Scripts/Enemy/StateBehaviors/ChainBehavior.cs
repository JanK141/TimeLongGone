using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using UnityEngine;

public class ChainBehavior : StateMachineBehaviour
{
    [SerializeField] private List<AICondition> contidiotsToChain;
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int test = 0;
        foreach (var condition in contidiotsToChain)
            if (condition.Check(animator.gameObject, PlayerScript.Instance.gameObject)) test++;
        if (test >= contidiotsToChain.Count)
        {
            animator.Play("AttackIdle", layerIndex);
        }
    }

}
