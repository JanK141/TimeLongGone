using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Enemy;
using Content.Scripts.Player;
using UnityEngine;

public class ChargeBehavior : StateMachineBehaviour
{
    [SerializeField] private LayerMask playerMask;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyScript.Instance.move.WalkTo(PlayerScript.Instance.transform.position);
        if (Physics.CheckSphere(animator.transform.position, 1.5f, playerMask))
        {
            animator.Play("ChargeHit", layerIndex);
        }
    }

}
