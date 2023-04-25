using FSMC;
using System;
using UnityEngine;

namespace FSMC
{
    [System.Serializable]
    public class FSMFloatParameter : FSMParameter, IComparable<float>
    {
        [SerializeField] private float _value = 0;
        private float? value = null;
        public float Value { get { value ??= _value; return (float)value; } set => _value = value; }

        public int CompareTo(float other)
        {
            int result = Value.CompareTo(other);
            if (result > 0) return 1;
            else if (result < 0) return -1;
            return 0;
        }
    }
}