using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttackHitbox : MonoBehaviour
    {
        [SerializeField] AttackStatus status;
        [SerializeField] float pushFactor;

        private void OnTriggerEnter(Collider other)
        {
            PlayerHitHandler player;
            if(other.TryGetComponent<PlayerHitHandler>(out player))
            {
                player.ProcessHit(status, GetComponent<Collider>(), pushFactor);
                gameObject.SetActive(false);
            }
        }


    }
}
