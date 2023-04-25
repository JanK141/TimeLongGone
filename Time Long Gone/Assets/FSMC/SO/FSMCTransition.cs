using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FSMC
{
    public class FSMCTransition : ScriptableObject
    {
        [SerializeField] public List<FSMCConditionWrapper> conditions;
        public FSMController StateMachine;
        public FSMCState OriginState;
        public FSMCState DestinationState;

        public FSMCState Evaluate()
        {
            if(conditions.Any(con=>con.conditions.All(c=>c.Check())))
                return DestinationState;
            return null;
        }
    }

    #region Conditions
    [Serializable]
    public class FSMCConditionWrapper
    {
        [SerializeReference]public List<FSMCCondition> conditions = new();
    }
    [Serializable]
    public abstract class FSMCCondition
    {
        public abstract bool Check();
    }
    [Serializable]
    public class FSMCFloatCondition : FSMCCondition
    {
        public float Value;
        public FSMFloatParameter parameter;
        public ComparisonType comparison;
        public override bool Check()
        {
            if (comparison == ComparisonType.NotEqual) return parameter.CompareTo(Value) != (int)ComparisonType.Equeal;
            return parameter.CompareTo(Value) == (int)comparison;
        }
    }
    [Serializable]
    public class FSMCIntegerCondition : FSMCCondition
    {
        public int Value;
        public FSMIntegerParameter parameter;
        public ComparisonType comparison;
        public override bool Check()
        {
            if (comparison == ComparisonType.NotEqual) return parameter.CompareTo(Value) != (int)ComparisonType.Equeal;
            return parameter.CompareTo(Value) == (int)comparison;
        }
    }
    [Serializable]
    public class FSMCBoolCondition : FSMCCondition
    {
        public bool Value;
        public FSMBoolParameter parameter;
        public ComparisonType comparison;
        public override bool Check()
        {
            if (comparison == ComparisonType.NotEqual) return parameter.CompareTo(Value) != (int)ComparisonType.Equeal;
            return parameter.CompareTo(Value) == (int)comparison;
        }
    }

    public enum ComparisonType
    {
        Lower=-1, Equeal=0, NotEqual=2, Greater=1
    }
    #endregion
}
