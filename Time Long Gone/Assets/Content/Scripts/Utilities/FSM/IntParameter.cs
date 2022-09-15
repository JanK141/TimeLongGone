using System.Collections;
using UnityEngine;

public class IntParameter : Parameter
{
    [SerializeField] public int value;

    public void Init(string name, int value, StateMachine parent)
    {
        base.Init(name, parent);
        this.value = value;
    }
}