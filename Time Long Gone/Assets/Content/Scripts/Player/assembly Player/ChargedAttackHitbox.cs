using System;
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
            //TODO
            // Try get component of enemy
            var dmg = player.combat.CalculateDashDamage();
            player.combat.ContinueCombo(1);
            // apply damage
            gameObject.SetActive(false);
        }
    }
}