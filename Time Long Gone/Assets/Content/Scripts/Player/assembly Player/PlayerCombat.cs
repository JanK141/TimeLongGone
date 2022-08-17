using System;
using UnityEngine;

namespace Content.Scripts.Player.assembly_Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] public PlayerVariables variables;
        [SerializeField] public LayerMask enemy;

        private int _combo = 0;
        private float _timeout = 0;


        private event Action OnHit;
        event Action<int> OnCombo;
        event Action<int> OnFinisher;


        private void Hit() => OnHit += Hit;
        private void Finisher(int var) => OnFinisher += Finisher;
        private void ContinueCombo(int var) => OnCombo += ContinueCombo;
    }
}