using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variable/Float")]
public class FloatVariable : ScriptableObject
{
    [SerializeField] private float variable;
    private float origin = 0f;
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
    public float OriginalValue {get => origin;}

    public void Reset() => origin = variable;
}
