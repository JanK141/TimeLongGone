namespace Content.Scripts.Player.States
{
    public interface IPlayerState
    {
        void OnstateEnter();
        void OnStateExit();
        void Tick();
        IPlayerState Evalueate();
    }
}