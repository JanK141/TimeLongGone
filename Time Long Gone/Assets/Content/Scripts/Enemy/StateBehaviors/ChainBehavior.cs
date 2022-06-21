using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.Enemy.AI_Conditions.Templates;
using Content.Scripts.Player;
using UnityEngine;

public class ChainBehavior : StateMachineBehaviour
{
    [SerializeField] private List<AICondition> contidiotsToChain;
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var test = contidiotsToChain.Count(condition => condition.Check(animator.gameObject, PlayerScript.Instance.gameObject));
        if (test >= contidiotsToChain.Count) animator.Play("AttackIdle", layerIndex);
    }

}
