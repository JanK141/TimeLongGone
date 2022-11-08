namespace Player.States
{
    public class Idle:IPlayerState
    {
        public Player player { get; set; }

        public virtual void OnStateEnter(bool playAnimation)
        {
        }

        public virtual void OnStateExit()
        {
        }

        public virtual void Tick()
        {
        }

        public virtual IPlayerState Evaluate()
        {
            var input = player.inputContext;
            //DASH
            if (input == InputIntermediary.InputContext.Dash && player.CanDash)
            {
                player.inputContext = InputIntermediary.InputContext.Nothing;
                return player.DASH_STATE;
            }
            //BLOCK
            if (input == InputIntermediary.InputContext.BlockStarted && player.CanBlock) return player.BLOCK_STATE;
            //JUMP
            if (input == InputIntermediary.InputContext.Jump && player.IsGrounded)
            {
                player.inputContext = InputIntermediary.InputContext.Nothing;
                return player.JUMP_STATE;
            }
            //FINISHER
            if (input == InputIntermediary.InputContext.FinisherStarted) return player.FINISHER_STATE;
            //CHARGING
            if (input == InputIntermediary.InputContext.ChargeStarted) return player.CHARGING_STATE;
            //ATTACK
            if (input == InputIntermediary.InputContext.Attack && player.CanAttack)
            {
                player.inputContext = InputIntermediary.InputContext.Nothing;
                return player.ATTACK_STATE;
            }
            //STUN
            if (input == InputIntermediary.InputContext.Stun)
            {
                player.inputContext = InputIntermediary.InputContext.Nothing;
                return player.STUN_STATE;
            }


            return null;
        }
    }
}