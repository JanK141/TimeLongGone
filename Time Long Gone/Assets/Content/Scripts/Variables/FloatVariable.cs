using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variable/Float")]
public class FloatVariable : ScriptableObject
{
    [SerializeField] private float variable;

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
}
