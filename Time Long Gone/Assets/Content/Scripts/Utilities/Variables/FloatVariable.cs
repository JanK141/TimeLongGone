using System;
using UnityEngine;

namespace Content.Scripts.Variables
{
    [CreateAssetMenu(menuName = "Variable/Float")]
    public class FloatVariable : ScriptableObject
    {
        [SerializeField] private float variable;
        private float _value = 0f;
        public event Action OnValueChange;

        public float Value
        {
            get => _value;
            set
            {
                _value = value;
                OnValueChange?.Invoke();
            }
        }

        /// <summary>
        /// The unmodified value that variable was initialized with in inspector.
        /// </summary>
        public float OriginalValue => variable;

        /// <summary>
        /// Resets value used at runtime (modifiable one) to value that is set in inspector (origin).
        /// Use it whenever you want to make sure that variable is in its default state.
        /// </summary>
        public void ResetToOrigin() => _value = variable;

        private void OnValidate() => ResetToOrigin();
    }
}
