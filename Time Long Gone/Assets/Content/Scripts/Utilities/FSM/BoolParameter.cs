using System.Collections;
using UnityEngine;

public class BoolParameter : Parameter
{
    [SerializeField] public bool value;

    public void Init(string name, bool value, StateMachine parent)
    {
        base.Init(name, parent);
        this.value = value;
    }
}