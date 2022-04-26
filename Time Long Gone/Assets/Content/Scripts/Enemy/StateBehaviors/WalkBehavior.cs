using Content.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Enemy;
using UnityEngine;

public class WalkBehavior : StateMachineBehaviour
{
    [SerializeField] private List<TriggerAndConditions> transitions;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        foreach (var t in transitions)
        {
            int test = 0;
            foreach (var condition in t.Conditions)
                if (condition.Check(animator.gameObject, PlayerScript.Instance.gameObject)) test++;
            if (test >= t.Conditions.Count)
            {
                animator.SetTrigger(t.Trigger);
                return;
            }
        }

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyScript.Instance.move.WalkTo(PlayerScript.Instance.transform.position);
    }

    [System.Serializable]
    private class TriggerAndConditions
    {
        public string Trigger;
        public List<AICondition> Conditions;
    }

}
