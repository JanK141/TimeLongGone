using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using UnityEngine;

public class AttackBehavior : StateMachineBehaviour
{
    [SerializeField] private List<TriggerAndConditions> transitions;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        foreach (var t in transitions)
        {
            int test = 0;
            foreach(var condition in t.Conditions)
                if(condition.Check(animator.gameObject, PlayerScript.Instance.gameObject)) test++;
            if (test >= t.Conditions.Count)
            {
                animator.SetTrigger(t.Trigger);
                return;
            }
        }

    }


    [System.Serializable]
    private class TriggerAndConditions
    {
        public string Trigger;
        public List<AICondition> Conditions;
    }
}
