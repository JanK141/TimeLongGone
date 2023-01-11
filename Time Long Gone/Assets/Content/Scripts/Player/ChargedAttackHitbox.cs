using Enemy;
using UnityEngine;

namespace Player
{
    public class ChargedAttackHitbox : MonoBehaviour
    {
        private Player player;

        void Start()
        {
            player = GetComponentInParent<Player>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponentInParent<IEnemy>();
            if(enemy != null && enemy.Status != EnemyStatus.Untouchable)
            {
                var dmg = player.combat.CalculateDashDamage();
                player.combat.ContinueCombo(1);
                enemy.ReceiveHit(dmg);
                gameObject.SetActive(false);
            }
        }
    }
}