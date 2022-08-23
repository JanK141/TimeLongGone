using UnityEngine;

namespace Player.States
{
    public class Stun : Interruptible
    {
        public StunAttackHitBox hitBox { get; set; }

        private Vector3 direction;

        public override void OnStateEnter()
        {
            hitBox.gameObject.SetActive(true);
            player.move = player.MoveGravityOnly;
            player.rotate = () => { };
            direction = player.velocity / player.SpeedFactor;
            player.animator.Play("StunAttack");
        }

        public override void OnStateExit()
        {
            hitBox.gameObject.SetActive(false);
            player.move = player.MoveNormal;
            player.rotate = player.InstaRotate;
        }

        public override void Tick()
        {
            player.velocity.x += direction.x * player.SpeedFactor;
            player.velocity.z += direction.z * player.SpeedFactor;
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