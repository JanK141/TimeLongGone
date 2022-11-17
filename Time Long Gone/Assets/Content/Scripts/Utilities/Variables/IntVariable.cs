using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variable/Int")]
public class IntVariable : ScriptableObject
{
    [SerializeField] private int variable;
    public int _value = 0;
    public event Action OnValueChange;

    public int Value
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
    public int OriginalValue => variable;

    /// <summary>
    /// Resets value used at runtime (modifiable one) to value that is set in inspector (origin).
    /// Use it whenever you want to make sure that variable is in its default state.
    /// </summary>
    public void ResetToOrigin() => _value = variable;

    private void OnValidate() => ResetToOrigin();

}
