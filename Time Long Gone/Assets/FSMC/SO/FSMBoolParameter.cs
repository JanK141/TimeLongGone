using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FSMC
{
    [System.Serializable]
    public class FSMBoolParameter : FSMParameter, IComparable<bool>
    {
        [SerializeField] private bool _value = false;
        private bool? value = null;
        public bool Value { get { value ??= _value; return (bool)value; } set => _value = value; }

        public int CompareTo(bool other)
        {
            int result = Value.CompareTo(other);
            if (result > 0) return 1;
            else if (result < 0) return -1;
            return 0;
        }
    }
}
