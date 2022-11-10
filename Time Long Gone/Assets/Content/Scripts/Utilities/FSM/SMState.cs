using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class SMState : ScriptableObject
{
    [SerializeField] public string stateName;
    [SerializeField] public List<SMTransition> transitions = new List<SMTransition>();
    [SerializeField] public StateMachine parent;
    [SerializeField] public List<SMAction> OnEnter = new List<SMAction>();
    [SerializeField] public List<SMAction> OnUpdate = new List<SMAction>();
    [SerializeField] public List<SMAction> OnExit = new List<SMAction>();
    public float timeInState;

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

    public void StateEnter(MonoBehaviour executer)
    {
        timeInState = 0;
        foreach (var a in OnEnter)
        {
            MethodInfo delegateInfo = Type.GetType(a.Executer).GetMethod(a.MethodName);
            string[] stringparams = a.Parameters.Split(',');
            ParameterInfo[] p = delegateInfo.GetParameters();
            System.Object[] passParams = new System.Object[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                switch (p[i].ParameterType.Name)
                {
                    case "Int32":
                        passParams[i] = int.Parse(stringparams[i]);
                        break;
                    case "String":
                        passParams[i] = stringparams[i];
                        break;
                    case "Single":
                        passParams[i] = float.Parse(stringparams[i], CultureInfo.InvariantCulture);
                        break;
                    case "Boolean":
                        passParams[i] = bool.Parse(stringparams[i]);
                        break;
                    default:
                        passParams[i] = stringparams[i];
                        break;
                }
            }
            if (a.Executer == typeof(StateMachine).Name) delegateInfo.Invoke(parent, passParams);
            else delegateInfo.Invoke(executer, passParams);
        }
    }

    public void StateUpdate(MonoBehaviour executer)
    {
        timeInState += Time.deltaTime;
        foreach(var a in OnUpdate)
        {
            MethodInfo delegateInfo = Type.GetType(a.Executer).GetMethod(a.MethodName);
            string[] stringparams = a.Parameters.Split(',');
            ParameterInfo[] p = delegateInfo.GetParameters();
            System.Object[] passParams = new System.Object[p.Length];
            for(int i = 0; i < p.Length; i++)
            {
                switch(p[i].ParameterType.Name){
                    case "Int32":
                        passParams[i] = int.Parse(stringparams[i]);
                        break;
                    case "String":
                        passParams[i] = stringparams[i];
                        break;
                    case "Single":
                        passParams[i] = float.Parse(stringparams[i], CultureInfo.InvariantCulture);
                        break;
                    case "Boolean":
                        passParams[i] = bool.Parse(stringparams[i]);
                        break;
                    default:
                        passParams[i] = stringparams[i];
                        break;
                }
            }
            if (a.Executer == typeof(StateMachine).Name) delegateInfo.Invoke(parent, passParams);
            else delegateInfo.Invoke(executer, passParams);
        }
    }

    public void StateExit(MonoBehaviour executer)
    {
        foreach (var a in OnExit)
        {
            MethodInfo delegateInfo = Type.GetType(a.Executer).GetMethod(a.MethodName);
            string[] stringparams = a.Parameters.Split(',');
            ParameterInfo[] p = delegateInfo.GetParameters();
            System.Object[] passParams = new System.Object[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                switch (p[i].ParameterType.Name)
                {
                    case "Int32":
                        passParams[i] = int.Parse(stringparams[i]);
                        break;
                    case "String":
                        passParams[i] = stringparams[i];
                        break;
                    case "Single":
                        passParams[i] = float.Parse(stringparams[i], CultureInfo.InvariantCulture);
                        break;
                    case "Boolean":
                        passParams[i] = bool.Parse(stringparams[i]);
                        break;
                    default:
                        passParams[i] = stringparams[i];
                        break;
                }
            }
            if (a.Executer == typeof(StateMachine).Name) delegateInfo.Invoke(parent, passParams);
            else delegateInfo.Invoke(executer, passParams);
        }
    }

    [Serializable]
    public class SMAction
    {
        [SerializeField] public string Executer;
        [SerializeField] public string MethodName;
        [SerializeField] public string Parameters;
    }
}
