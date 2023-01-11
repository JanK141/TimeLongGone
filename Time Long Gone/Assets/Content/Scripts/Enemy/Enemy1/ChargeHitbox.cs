using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy {
    public class ChargeHitbox : EnemyAttackHitbox
    {
        [SerializeField] LayerMask obstacleMask;
        [SerializeField] Enemy1 enemy;

        private new void OnTriggerEnter(Collider other)
        {
            if(((1<<other.gameObject.layer) & obstacleMask) != 0)
            {
                enemy.PlayAnimation("ChargeHit", 0.1f);
                enemy.StopAI(2f);
                enemy.ChargeHit();
                if(setInactiveAfterCol) gameObject.SetActive(false);
            }else if (other.GetComponent<PlayerHitHandler>() != null)
            {
                enemy.PlayAnimation("ChargeHit", 0.1f);
                enemy.StopAI(2f);
                enemy.ChargeHit();
                base.OnTriggerEnter(other);
            }
        }

    }
}
