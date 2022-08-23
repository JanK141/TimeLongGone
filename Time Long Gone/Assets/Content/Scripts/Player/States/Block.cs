using UnityEngine;

namespace Player.States
{
    public class Block : IPlayerState
    {
        public Player player { get; set; }

        public virtual void OnStateEnter()
        {
            player.move = player.MoveSlow;
            player.BlockTime = Time.time;
            player.IsBlocking = true;
        }

        public virtual void OnStateExit()
        {
            player.move = player.MoveNormal;
            player.ResetBlock();
            player.IsBlocking = false;
        }

        public virtual void Tick()
        {
            
        }

        public virtual IPlayerState Evaluate()
        {
            if (player.inputContext == InputIntermediary.InputContext.BlockCanceled) return player.IDLE_STATE;
            return null;
        }
    }
}