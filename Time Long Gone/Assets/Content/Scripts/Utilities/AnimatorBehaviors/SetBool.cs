using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBool : StateMachineBehaviour
{
    [SerializeField] private List<BoolToSet> BoolsToSet;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach(BoolToSet value in BoolsToSet)
        {
            animator.SetBool(value.Parameter, value.value);
        }
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (BoolToSet value in BoolsToSet)
        {
            animator.SetBool(value.Parameter, value.value);
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (BoolToSet value in BoolsToSet)
        {
            animator.SetBool(value.Parameter, !value.value);
        }
    }

    [Serializable]
    private struct BoolToSet
    {
        public string Parameter;
        public bool value;
    }
}
