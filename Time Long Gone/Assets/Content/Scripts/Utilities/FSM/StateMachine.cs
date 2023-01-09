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
    [SerializeField] public string executorType = "UnityEngine.MonoBehaviour, UnityEngine.CoreModule";

    private SMState _currState;

    public void Start()
    {
        foreach(Parameter parameter in parameters)
        {
            if (parameter is FloatParameter) (parameter as FloatParameter).value = 0f;
            else if (parameter is IntParameter) (parameter as IntParameter).value = 0;
            else (parameter as BoolParameter).value = false;
        }
        foreach (SMState state in states)
        {
            state.Start();
        }
        foreach (SMTransition transition in transitions)
        {
            transition.Start();
        }

        _currState = initialState;
    }

    private SMState _transitionTo;
    public void Tick(MonoBehaviour executer)
    {
        if(_transitionTo != null)
        {
            _currState = _transitionTo;
            _transitionTo = null;
            _currState.StateEnter(executer);
        }
        _currState.StateUpdate(executer);
        SMState state = null;
        foreach (SMTransition transition in transitions)
        {
            if (transition.Check() && transition.to != _currState) { 
                state = transition.to;
                break;
            }
        }
        if(state==null)
            state =_currState.Evaluate();
        if (state != null)
        {
            _currState.StateExit(executer);
            _transitionTo = state;
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
    public SMState GetCurrentState()
    {
        return _currState;
    }
    public void SetCurrentState(SMState state, MonoBehaviour executer)
    {
        if (state.parent != this) return;
        _currState.StateExit(executer);
        _currState = state;
        _currState.StateEnter(executer);
    }
}

