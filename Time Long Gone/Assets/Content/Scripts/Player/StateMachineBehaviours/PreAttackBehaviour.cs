using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using UnityEngine;

public class PreAttackBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerScript.Instance.combat.InvokeAttackReset(PlayerScript.Instance.anim.GetCurrentAnimatorStateInfo(0).length+0.05f);
    }
}
