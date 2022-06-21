using Content.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.Enemy;
using Content.Scripts.Enemy.AI_Conditions.Templates;
using UnityEngine;

public class WalkBehavior : StateMachineBehaviour
{
    [SerializeField] private List<TriggerAndConditions> transitions;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var t in from t in transitions
                 let test = t.Conditions.Count(condition =>
                     condition.Check(animator.gameObject, PlayerScript.Instance.gameObject))
                 where test >= t.Conditions.Count
                 select t)
        {
            animator.SetTrigger(t.Trigger);
            return;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) =>
        EnemyScript.Instance.move.WalkTo(PlayerScript.Instance.transform.position);

    [System.Serializable]
    private class TriggerAndConditions
    {
        public string Trigger;
        public List<AICondition> Conditions;
    }
}