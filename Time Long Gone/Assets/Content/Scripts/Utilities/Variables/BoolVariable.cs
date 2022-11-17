using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.Variables
{
    [CreateAssetMenu(menuName = "Variable/Bool")]
    public class BoolVariable : ScriptableObject
    {
        [SerializeField] private bool variable;
        private bool _value = false;
        public event Action OnValueChange;

        public bool Value
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
        public bool OriginalValue => variable;

        /// <summary>
        /// Resets value used at runtime (modifiable one) to value that is set in inspector (origin).
        /// Use it whenever you want to make sure that variable is in its default state.
        /// </summary>
        public void ResetToOrigin() => _value = variable;

        private void OnValidate() => ResetToOrigin();

    }
}
