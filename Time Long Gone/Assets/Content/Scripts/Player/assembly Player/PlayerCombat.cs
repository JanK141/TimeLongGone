using System;
using UnityEngine;

namespace Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] public PlayerVariables variables;
        [SerializeField] public LayerMask enemy;

        private int _combo = 0;
        private float _timeout = 0;


        public static event Action OnHit;
        public static event Action<int> OnCombo;
        public static event Action<int> OnFinisher;


        private void Hit() => OnHit += Hit;
        private void Finisher(int var) => OnFinisher += Finisher;
        private void ContinueCombo(int var) => OnCombo += ContinueCombo;
    }
}