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
            //TODO
            // Try get compnent of enemy
            player.combat.ContinueCombo(1);
            // apply stun
            gameObject.SetActive(false);
        }
    }
}