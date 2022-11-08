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
        private bool _origin = false;
        public event Action OnValueChange;

        public bool Value
        {
            get => variable;
            set
            {
                variable = value;
                OnValueChange?.Invoke();
            }
        }
        public bool OriginalValue => _origin;

        public void Reset() => _origin = variable;
    }
}
