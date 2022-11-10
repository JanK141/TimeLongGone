using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variable/Int")]
public class IntVariable : ScriptableObject
{
    [SerializeField] private int variable;
    public int origin = 0;
    public event Action OnValueChange;

    public int Value
    {
        get => variable;
        set
        {
            variable = value;
            OnValueChange?.Invoke();
        }
    }

    public int OriginalValue { get => origin;}

    public void Reset() => origin = variable;
}
