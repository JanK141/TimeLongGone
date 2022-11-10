using UnityEngine;

namespace Player.States
{
    public class Jump : IPlayerState
    {
        public float jumpHeight { get; set; }
        public Player player { get; set; }

        public virtual void OnStateEnter(bool playAnimation)
        {
            if (player.velocity.y <= 0)
            {
                player.velocity.y = Mathf.Sqrt(jumpHeight * -2f * player.Gravity);
                if(playAnimation)player.animator.Play("Jump");
            }
        }

        public virtual void OnStateExit()
        {
        }

        public virtual void Tick()
        {
        }

        public virtual IPlayerState Evaluate()
        {
            if (player.velocity.y <= 0) return player.IDLE_STATE;
            return null;
        }
    }
}