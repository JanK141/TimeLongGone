using System;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// This class manages players combo and damage calculations.
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {

        private int _combo = 0;
        private float _timeout = 0;
        private Player player;
        private PlayerVariables variables;
        private LayerMask enemy;

        #region Events
        /// <summary>
        /// Informs about every successful hit. Also tells if attack was last in animations chain.
        /// </summary>
        public static event Action<bool> OnHit;
        /// <summary>
        /// Informs about any changes in combo meter.
        /// </summary>
        public static event Action<int> OnCombo;
        /// <summary>
        /// Informs about every successfully performed finisher with combo meter atwich it was performed.
        /// </summary>
        public static event Action<int> OnFinisher;
        #endregion

        void Start()
        {
            player = GetComponent<Player>();
            variables = player.variables;
            enemy = player.enemy;
        }

        void Update()
        {
            if(_combo <= 0) return;
            _timeout -= Time.deltaTime;
            if(_timeout <=0) ContinueCombo(-1);
        }

        internal void Hit(bool lastInChain = false)
        {
            if(!Physics.CheckSphere(transform.position + transform.forward * variables.attackDistance, variables.attackRadius, enemy)) return;

            OnHit?.Invoke(lastInChain);
            ContinueCombo(1);
            float baseDmg = lastInChain ? variables.baseDamage * 1.5f : variables.baseDamage;
            float damage = baseDmg + baseDmg * _combo * variables.comboMultiplier;
            //TODO enemy receive dmg
        }

        internal void Finisher()
        {
            if (!Physics.CheckSphere(transform.position + transform.forward * variables.attackDistance, variables.attackRadius, enemy)) return;

            OnFinisher?.Invoke(_combo);
            float damage = variables.baseDamage + variables.baseDamage * _combo * variables.comboMultiplier * variables.finisherMultiplier;
            ContinueCombo(-1);
            //TODO enemy receive dmg
        }

        internal float CalculateDashDamage()
        {
            return (variables.baseDamage + variables.baseDamage * _combo * variables.comboMultiplier) *
                player.ChargeAttackMultiplier;
        }

        /// <summary>
        /// Positive number raises combo up, negative deletes combo and 0 simply continues combo without raising it (resets timer).
        /// </summary>
        /// <param name="value"></param>
        internal void ContinueCombo(int value)
        {
            if (value > 0) _combo++;
            if (value >= 0) _timeout = variables.comboTimeout;
            else _combo = 0;
                
            OnCombo?.Invoke(_combo);
        }
    }
}