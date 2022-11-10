using UnityEngine;

namespace Player.States
{
    public class Stun : Interruptible
    {
        public StunAttackHitBox hitBox { get; set; }

        private Vector3 direction;

        public override void OnStateEnter(bool playAnimation)
        {
            hitBox.gameObject.SetActive(true);
            player.move = player.MoveGravityOnly;
            player.rotate = () => { };
            direction = player.velocity / player.SpeedFactor;
            if(playAnimation)player.animator.Play("StunAttack");
        }

        public override void OnStateExit()
        {
            hitBox.gameObject.SetActive(false);
            player.move = player.MoveNormal;
            player.rotate = player.InstaRotate;
        }

        public override void Tick()
        {
            var dir = direction * Mathf.Clamp(1-player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 0.2f, 1f);
            player.velocity.x += dir.x * player.SpeedFactor;
            player.velocity.z += dir.z * player.SpeedFactor;
        }

        public override IPlayerState Evaluate()
        {
            var state = base.Evaluate();
            if (state != null) return state;
            if (player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95) return player.IDLE_STATE;
            return null;
        }
    }
}