﻿using UnityEngine;

namespace Player.States
{
    public class DashAttack : Dash
    {
        public float maxDistance { get; set; }
        public ChargedAttackHitbox hitBox { get; set; }
        public LayerMask enemy { get; set; }


        public override void OnStateEnter(bool playAnimation)
        {
            hitBox.gameObject.SetActive(true);
            distance = (player.ChargeAttackMultiplier / player.variables.maxChargeMult) * maxDistance;
            base.OnStateEnter(playAnimation);
            if(playAnimation)player.animator.Play("DashAttack");
        }

        public override void OnStateExit()
        {
            hitBox.gameObject.SetActive(false);
            base.OnStateExit();
        }

        public override void Tick()
        {
            base.Tick();
        }

        public override IPlayerState Evaluate()
        {
            if (time >= dashTime / player.SpeedFactor) return player.IDLE_STATE;
            return null;
        }
    }
}