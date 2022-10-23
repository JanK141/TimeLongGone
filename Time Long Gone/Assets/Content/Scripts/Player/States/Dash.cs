using UnityEngine;

namespace Player.States
{
    public class Dash : IPlayerState
    {
        public float distance { get; set; }
        public float iframesTime { get; set; }
        public float dashTime { get; set; }

        private Vector3 direction;
        protected float time;

        public Player player { get; set; }

        public virtual void OnStateEnter()
        {
            time = 0;
            direction = (player._moveDirection.Equals(Vector3.zero)
                ? player.transform.forward
                : player._moveDirection.normalized) * distance;
            player.move = () => { };
            player.rotate = () => { };
            player.IsInvincible = true;
            player.animator.Play("Dash");
            if (player.combat.enemy != null) Physics.IgnoreLayerCollision(player.gameObject.layer, (player.combat.enemy as MonoBehaviour).gameObject.layer, true);
        }

        public virtual void OnStateExit()
        {
            player.move = player.MoveNormal;
            player.rotate = player.InstaRotate;
            player.ResetDash();
            player.IsInvincible = false;
            if (player.combat.enemy != null) Physics.IgnoreLayerCollision(player.gameObject.layer, (player.combat.enemy as MonoBehaviour).gameObject.layer, false);
        }

        public virtual void Tick()
        {
            if (player.IsInvincible && time >= iframesTime / player.SpeedFactor) player.IsInvincible = false;
            time += Time.deltaTime;
            player.velocity = direction / (dashTime / player.SpeedFactor);
        }

        public virtual IPlayerState Evaluate()
        {
            if (time >= dashTime / player.SpeedFactor) return player.IDLE_STATE;
            if (player.inputContext == InputIntermediary.InputContext.Stun)
            {
                player.inputContext = InputIntermediary.InputContext.Nothing;
                return player.STUN_STATE;
            }
            return null;
        }
    }
}