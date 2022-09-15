using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Parameter : ScriptableObject
{
    [SerializeField] public string paramName;
    [HideInInspector]public StateMachine parent;

    protected void Init(string name, StateMachine parent)
    {
        this.paramName = name;
        this.parent = parent;
    }
}
