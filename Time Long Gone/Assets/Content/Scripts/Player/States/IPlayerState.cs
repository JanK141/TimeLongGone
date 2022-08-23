namespace Player.States
{
    public interface IPlayerState
    {
        public Player player { get; set; }

        public void OnStateEnter();
        public void OnStateExit();
        public void Tick();
        public IPlayerState Evaluate();
    }
}