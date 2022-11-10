using System.Collections;
using UnityEngine;

public class FloatParameter : Parameter
{
    [SerializeField] public float value;

    public void Init(string name, float value, StateMachine parent)
    {
        base.Init(name, parent);
        this.value = value;
    }
}