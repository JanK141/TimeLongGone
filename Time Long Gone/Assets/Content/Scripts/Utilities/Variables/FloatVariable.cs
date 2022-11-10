using System;
using UnityEngine;

namespace Content.Scripts.Variables
{
    [CreateAssetMenu(menuName = "Variable/Float")]
    public class FloatVariable : ScriptableObject
    {
        [SerializeField] private float variable;
        private float _origin = 0f;
        public event Action OnValueChange;

        public float Value
        {
            get => variable;
            set
            {
                variable = value;
                OnValueChange?.Invoke();
            }
        }
        public float OriginalValue => _origin;

        public void Reset() => _origin = variable;
    }
}
