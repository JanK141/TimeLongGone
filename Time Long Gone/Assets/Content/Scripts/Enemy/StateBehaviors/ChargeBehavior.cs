using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Enemy;
using Content.Scripts.Player;
using UnityEngine;

public class ChargeBehavior : StateMachineBehaviour
{
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float hitRadius = 1.5f;
    [SerializeField] private float distanceToLockDirection = 3f;
    [SerializeField] private float chargeTimeAfterLock = 2f;

    private bool isLocked = false;
    private float chargeTime = 0f;
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!isLocked && (PlayerScript.Instance.transform.position - animator.transform.position).magnitude <= distanceToLockDirection)
            isLocked=true;

        if (!isLocked)
            EnemyScript.Instance.move.WalkTo(PlayerScript.Instance.transform.position);
        else if (chargeTime < chargeTimeAfterLock)
        {
            Transform parent = animator.gameObject.GetComponentInParent<Transform>();
            EnemyScript.Instance.move.WalkTo(parent.position + parent.forward * 2);
            chargeTime += Time.deltaTime;
        }

        if (chargeTime >= chargeTimeAfterLock)
        {
            Transform parent = animator.gameObject.GetComponentInParent<Transform>();
            EnemyScript.Instance.move.WalkTo(parent.position + parent.forward);
            //animator.CrossFade("Idle", 0.5f);
            animator.Play("Idle");
        }
        Debug.Log("Charging");

        if (Physics.CheckSphere(animator.transform.position, hitRadius, hitMask))
        {
            if ((PlayerScript.Instance.transform.position - animator.transform.position).magnitude <= hitRadius + 1.5f)
            {
                PlayerScript.Instance.hit.ReceiveHit();
                Debug.Log("Charge hit");
            }

            animator.Play("ChargeHit", layerIndex);
        }
    }

}
