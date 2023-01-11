using Enemy;
using System;
using UnityEngine;

namespace Player
{
    public class StunAttackHitBox : MonoBehaviour
    {
        private Player player;
        public bool IsStunHited { get; private set; }

        void Start()
        {
            player = GetComponentInParent<Player>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponentInParent<IEnemy>();
            if (enemy != null) IsStunHited = true;
            if (enemy is { Status: EnemyStatus.Vulnerable })
            {
                player.combat.ContinueCombo(1);
                enemy.ReceiveStun();
                gameObject.SetActive(false);
            }

            IsStunHited = false;
        }
    }
}