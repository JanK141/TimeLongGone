using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variable/Int")]
public class IntVariable : ScriptableObject
{
    [SerializeField] private int variable;

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

}
