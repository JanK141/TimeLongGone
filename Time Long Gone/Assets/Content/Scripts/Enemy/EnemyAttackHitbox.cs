using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttackHitbox : MonoBehaviour
    {
        [SerializeField] AttackStatus status;
        [SerializeField] [Range(0,1)] float pushFactor;
        [SerializeField] bool setInactiveAfterCol = true;

        private void OnTriggerEnter(Collider other)
        {
            PlayerHitHandler player;
            if(other.TryGetComponent<PlayerHitHandler>(out player))
            {
                player.ProcessHit(status, GetComponent<Collider>(), pushFactor);
                if(setInactiveAfterCol)gameObject.SetActive(false);
            }
        }


    }
}
