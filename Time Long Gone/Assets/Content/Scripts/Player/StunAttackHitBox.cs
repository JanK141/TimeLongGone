using Enemy;
using System;
using UnityEngine;

namespace Player
{
    public class StunAttackHitBox : MonoBehaviour
    {
        private Player player;

        void Start()
        {
            player = GetComponentInParent<Player>();
        }
        private void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponentInParent<IEnemy>();
            if(enemy != null && enemy.Status == EnemyStatus.Vulnerable)
            {
                player.combat.ContinueCombo(1);
                enemy.ReceiveStun();
                gameObject.SetActive(false);
            }
        }
    }
}