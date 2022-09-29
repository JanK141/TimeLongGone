﻿namespace Player.States
{
    public class Dead : IPlayerState
    {
        public Player player { get; set; }

        public virtual void OnStateEnter()
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
            return null;
        }
    }
}