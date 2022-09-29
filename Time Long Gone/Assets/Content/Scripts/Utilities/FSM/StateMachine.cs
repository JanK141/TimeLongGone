using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "State Machine")]
public class StateMachine : ScriptableObject
{
    [SerializeField] public List<Parameter> parameters = new List<Parameter>();
    [SerializeField] public List<SMState> states = new List<SMState>();
    [SerializeField] public SMState initialState;
    [SerializeField] public List<SMTransition> transitions;

    private SMState _currState;

    public void Start()
    {
        foreach (SMState state in states)
        {
            state.Start();
        }

        _currState = initialState;
    }

    public void Update()
    {
        _currState.StateUpdate();
        SMState state = null;
        foreach (SMTransition transition in transitions)
        {
            if (transition.Check()) { 
                state = transition.to;
                break;
            }
        }
        if(state==null)
            state =_currState.Evaluate();
        if (state != null)
        {
            _currState.StateExit();
            _currState = state;
            _currState.StateEnter();
        }
    }

    public void SetFloat(string name, float value)
    {
        (parameters.SingleOrDefault(p => p.paramName == name) as FloatParameter).value = value;
    }

    public float GetFloat(string name)
    {
        return (parameters.SingleOrDefault(p => p.paramName == name) as FloatParameter).value;
    }

    public void SetInt(string name, int value)
    {
        (parameters.SingleOrDefault(p => p.paramName == name) as IntParameter).value = value;
    }
    public int GetInt(string name)
    {
        return (parameters.SingleOrDefault(p => p.paramName == name) as IntParameter).value;
    }
    public void SetBool(string name, bool value)
    {
        (parameters.SingleOrDefault(p => p.paramName == name) as BoolParameter).value = value;
    }
    public bool GetBool(string name)
    {
        return (parameters.SingleOrDefault(p => p.paramName == name) as BoolParameter).value;
    }
}

