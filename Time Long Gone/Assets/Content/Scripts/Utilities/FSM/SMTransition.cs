using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class SMTransition : ScriptableObject
{
    [SerializeField] public string transitionName;
    [SerializeField] public SMState from;
    [SerializeField] public SMState to;
    [SerializeField] public StateMachine parent;
    [SerializeField] public List<SMCondition> conditions = new List<SMCondition>();

    public void Init(SMState from, SMState to, StateMachine parent)
    {
        this.@from = from;
        this.to = to;
        this.parent = parent;
        transitionName = from.stateName + " -> " + to.stateName;
    }
    public void Start()
    {
        foreach (SMCondition condition in conditions)
        {
            condition.Init();
        }
    }

    public bool Check()
    {
        foreach (SMCondition condition in conditions)
        {
            if (!condition.Evaluate.Invoke()) return false;
        }
        return true;
    }
}
[Serializable]
public class SMCondition
{
    public enum ParameterType
    {
        Int,
        Float,
        Bool
    }

    public enum Comparison
    {
        Lower,
        LowerOrEqual,
        Equal,
        NotEqual,
        HigherOrEqual,
        Higher
    }
    [SerializeField] public Parameter parameter;
    [SerializeField] public ParameterType type;
    [SerializeField] public Comparison comparison;
    [SerializeField] public int intValue;
    [SerializeField] public float floatValue;
    [SerializeField] public bool boolValue;
    [SerializeField] public SMTransition parent;

    public Func<bool> Evaluate;

    public void Init()
    {
        if (type == ParameterType.Bool && !(comparison == Comparison.Equal || comparison == Comparison.NotEqual))
        {
            throw new Exception("Invalid Comparison on ParameterType Bool");
        }
        switch (comparison)
        {
            case Comparison.Lower:
                switch (type)
                {
                    case ParameterType.Int:
                        Evaluate = () => ((IntParameter) parameter).value < intValue;
                        break;
                    case ParameterType.Float:
                        Evaluate = () => ((FloatParameter)parameter).value < floatValue;
                        break;
                }
                break;
            case Comparison.LowerOrEqual:
                switch (type)
                {
                    case ParameterType.Int:
                        Evaluate = () => ((IntParameter)parameter).value <= intValue;
                        break;
                    case ParameterType.Float:
                        Evaluate = () => ((FloatParameter)parameter).value <= floatValue;
                        break;
                }
                break;
            case Comparison.Equal:
                switch (type)
                {
                    case ParameterType.Int:
                        Evaluate = () => ((IntParameter)parameter).value == intValue;
                        break;
                    case ParameterType.Float:
                        Evaluate = () => ((FloatParameter)parameter).value == floatValue;
                        break;
                    case ParameterType.Bool:
                        Evaluate = () => ((BoolParameter)parameter).value == boolValue;
                        break;
                }
                break;
            case Comparison.NotEqual:
                switch (type)
                {
                    case ParameterType.Int:
                        Evaluate = () => ((IntParameter)parameter).value != intValue;
                        break;
                    case ParameterType.Float:
                        Evaluate = () => ((FloatParameter)parameter).value != floatValue;
                        break;
                    case ParameterType.Bool:
                        Evaluate = () => ((BoolParameter)parameter).value != boolValue;
                        break;
                }
                break;
            case Comparison.HigherOrEqual:
                switch (type)
                {
                    case ParameterType.Int:
                        Evaluate = () => ((IntParameter)parameter).value >= intValue;
                        break;
                    case ParameterType.Float:
                        Evaluate = () => ((FloatParameter)parameter).value >= floatValue;
                        break;
                }
                break;
            case Comparison.Higher:
                switch (type)
                {
                    case ParameterType.Int:
                        Evaluate = () => ((IntParameter)parameter).value > intValue;
                        break;
                    case ParameterType.Float:
                        Evaluate = () => ((FloatParameter)parameter).value > floatValue;
                        break;
                }
                break;
        }
        
    }
}
