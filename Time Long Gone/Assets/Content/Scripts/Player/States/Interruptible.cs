namespace Player.States
{
    public class Interruptible : IPlayerState
        /*
        * Base class
        * checks if action
        * is being interrupted
        */
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
            if (input == InputIntermediary.InputContext.Dash && player.CanDash)
            {
                player.inputContext = InputIntermediary.InputContext.Nothing;
                return player.DASH_STATE;
            }

            if (input == InputIntermediary.InputContext.BlockStarted && player.CanBlock) return player.BLOCK_STATE;
            if (input == InputIntermediary.InputContext.Jump && player.IsGrounded)
            {
                player.inputContext = InputIntermediary.InputContext.Nothing;
                return player.JUMP_STATE;
            }

            return null;
        }
    }
}