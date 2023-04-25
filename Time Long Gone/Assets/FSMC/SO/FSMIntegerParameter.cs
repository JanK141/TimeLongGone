using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FSMC
{
    [System.Serializable]
    public class FSMIntegerParameter : FSMParameter, IComparable<int>
    {
        [SerializeField] private int _value = 0;
        private int? value = null;
        public int Value { get { value ??= _value; return (int)value; } set => _value = value; }

        public int CompareTo(int other)
        {
            int result = Value.CompareTo(other);
            if (result > 0) return 1;
            else if (result < 0) return -1;
            return 0;
        }
    }
}
