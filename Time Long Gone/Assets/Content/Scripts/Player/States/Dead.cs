namespace Player.States
{
    public class Dead : IPlayerState
    {
        public Player player { get; set; }

        public virtual void OnStateEnter(bool playAnimation)
        {
            if(playAnimation) player.animator.Play("Death");
        }

        public virtual void OnStateExit()
        {
        }

        public virtual void Tick()
        {
        }

        public virtual IPlayerState Evaluate()
        {
            return null;
        }
    }
}