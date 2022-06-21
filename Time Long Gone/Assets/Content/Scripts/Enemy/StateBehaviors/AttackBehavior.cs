using System.Collections.Generic;
using System.Linq;
using Content.Scripts.Player;
using UnityEngine;

public class AttackBehavior : StateMachineBehaviour
{
    [SerializeField] private List<TriggerAndConditions> transitions;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var t in from t in transitions
                 let test = t.conditions.Count(condition =>
                     condition.Check(animator.gameObject, PlayerScript.Instance.gameObject))
                 where test >= t.conditions.Count
                 select t)
        {
            animator.SetTrigger(t.trigger);
            return;
        }
    }


    [System.Serializable]
    private class TriggerAndConditions
    {
        public string trigger;
        public List<AICondition> conditions;
    }
}