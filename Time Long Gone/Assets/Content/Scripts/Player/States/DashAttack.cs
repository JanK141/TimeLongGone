using UnityEngine;

namespace Player.States
{
    public class DashAttack : Dash
    {
        public DashAttack()
        {
            PlayerCombat.OnCombo += UpdateCombo;
        }
        public float maxDistance { get; set; }
        public ChargedAttackHitbox hitBox { get; set; }
        public LayerMask enemy { get; set; }
        private int currentCombo = 0;


        public override void OnStateEnter()
        {
            hitBox.gameObject.SetActive(true);
            hitBox.damage =
                (player.variables.baseDamage + player.variables.baseDamage *
                    Mathf.Clamp(currentCombo, 0, player.variables.comboDmgCap) * player.variables.comboMultiplayer) *
                player.ChargeAttackMultiplier;
            distance = (player.ChargeAttackMultiplier / player.variables.maxChargeMult) * maxDistance;
            Physics.IgnoreLayerCollision(player.gameObject.layer, enemy.value, true);
            base.OnStateEnter();
            player.animator.Play("DashAttack");
        }

        public override void OnStateExit()
        {
            hitBox.gameObject.SetActive(false);
            Physics.IgnoreLayerCollision(player.gameObject.layer, enemy.value, false);
            base.OnStateExit();
        }

        public override void Tick()
        {
            base.Tick();
        }

        public override IPlayerState Evaluate()
        {
            if (time >= dashTime / player.SpeedFactor) return player.IDLE_STATE;
            return null;
        }

        private void UpdateCombo(int combo) => currentCombo = combo;

        ~DashAttack()
        {
            PlayerCombat.OnCombo -= UpdateCombo;
        }
    }
}