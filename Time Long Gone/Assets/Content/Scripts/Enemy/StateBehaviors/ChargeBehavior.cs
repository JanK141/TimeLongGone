using Content.Scripts.Player;
using UnityEngine;

namespace Content.Scripts.Enemy.StateBehaviors
{
    public class ChargeBehavior : StateMachineBehaviour
    {
        [SerializeField] private LayerMask hitMask;
        [SerializeField] private float hitRadius = 1.5f;
        [SerializeField] private float distanceToLockDirection = 3f;
        [SerializeField] private float chargeTimeAfterLock = 2f;

        private bool _isLocked;
        private float _chargeTime;
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            if (!_isLocked && (PlayerScript.Instance.transform.position - animator.transform.position).magnitude <= distanceToLockDirection)
                _isLocked=true;

            if (!_isLocked)
                EnemyScript.Instance.move.WalkTo(PlayerScript.Instance.transform.position);
            else if (_chargeTime < chargeTimeAfterLock)
            {
                var parent = animator.gameObject.GetComponentInParent<Transform>();
                EnemyScript.Instance.move.WalkTo(parent.position + parent.forward * 2);
                _chargeTime += Time.deltaTime;
            }

            if (_chargeTime >= chargeTimeAfterLock)
            {
                var parent = animator.gameObject.GetComponentInParent<Transform>();
                EnemyScript.Instance.move.WalkTo(parent.position + parent.forward);
                //animator.CrossFade("Idle", 0.5f);
                animator.Play("Idle");
            }
            Debug.Log("Charging");

            if (!Physics.CheckSphere(animator.transform.position, hitRadius, hitMask)) return;
        
            if ((PlayerScript.Instance.transform.position - animator.transform.position).magnitude <= hitRadius + 1.5f)
            {
                PlayerScript.Instance.hit.ReceiveHit();
                Debug.Log("Charge hit");
            }

            animator.Play("ChargeHit", layerIndex);
        }

    }
}
