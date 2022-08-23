using UnityEngine;

namespace Player.States
{
    public class Charging : Interruptible
    {
        public override void OnStateEnter()
        {
            player.move = player.MoveSlow;
            player.animator.Play("Charging");
            player.ChargeAttackMultiplier = 0;
        }

        public override void OnStateExit()
        {
            player.move = player.MoveNormal;
            player.ChargeAttackMultiplier = Mathf.Clamp(player.ChargeAttackMultiplier, 1f, player.variables.maxChargeMult);
        }

        public override void Tick()
        {
            player.ChargeAttackMultiplier += Time.deltaTime * player.SpeedFactor;
        }

        public override IPlayerState Evaluate()
        {
            var state = base.Evaluate();
            if (state != null) return state;
            if (player.inputContext == InputIntermediary.InputContext.ChargeCanceled) return player.DASHATTACK_STATE;
            return null;
        }
    }
}