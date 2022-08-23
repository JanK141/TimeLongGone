using UnityEngine;

namespace Player.States
{
    public class Attack : Interruptible
    {
        public float chainTime { get; set; }
        public float airborne { get; set; }

        private float time;
        private float tmpGravity; //Remembers it to restore after airborne

        public override void OnStateEnter()
        {
            time = 0;
            player.move = player.MoveSlow;
            player.rotate = player.SlowRotate;
            player.velocity = Vector3.zero;
            tmpGravity = player.Gravity;
            player.Gravity = 0;
            player.animator.SetTrigger("Attack");
            player.ResetAttack();
        }

        public override void OnStateExit()
        {
            player.move = player.MoveNormal;
            player.rotate = player.InstaRotate;
            player.Gravity = tmpGravity;
        }

        public override void Tick()
        {
            if (player.Gravity != 0 && time >= airborne * player.SpeedFactor) player.Gravity = tmpGravity;
            time += Time.deltaTime;
        }

        public override IPlayerState Evaluate()
        {
            var state = base.Evaluate();
            if (state != null) return state;
            if (player.CanAttack && player.inputContext == InputIntermediary.InputContext.Attack)
            {
                player.inputContext = InputIntermediary.InputContext.Nothing;
                return player.ATTACK_STATE;
            }
            if (time >= chainTime) return player.IDLE_STATE;
            return null;
        }
    }
}