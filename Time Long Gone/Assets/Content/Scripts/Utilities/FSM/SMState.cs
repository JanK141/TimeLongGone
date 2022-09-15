using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SMState : ScriptableObject
{
    [SerializeField] public string stateName;
    [SerializeField] public List<SMTransition> transitions;
    [SerializeField] public StateMachine parent;
    [SerializeField] public UnityEvent onStateEnter;
    [SerializeField] public UnityEvent onStateUpdate;
    [SerializeField] public UnityEvent onStateExit;

    public void Init(StateMachine parent, string name)
    {
        this.parent = parent;
        stateName = name;
    }

    public void Start()
    {
        foreach (SMTransition transition in transitions)
        {
            transition.Start();
        }
    }

    public SMState Evaluate()
    {
        foreach (SMTransition transition in transitions)
        {
            if (transition.Check()) return transition.to;
        }
        return null;
    }

    public void StateEnter()
    {
        onStateEnter?.Invoke();
    }

    public void StateUpdate()
    {
        onStateUpdate?.Invoke();
    }

    public void StateExit()
    {
        onStateExit?.Invoke();
    }
}
