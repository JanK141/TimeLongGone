using FSMC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public class FSMCExecuter : MonoBehaviour
{
    [SerializeField] private FSMController stateMachine;

    public FSMController StateMachine { get { return stateMachine; } set { if (value != null) { value.StartStateMachine(this); stateMachine = value; } } }





    void Start()
    {
        stateMachine.StartStateMachine(this);
    }

    void Update()
    {
        stateMachine.UpdateStateMachine(this);
    }



    public void SetFloat(string name, float value)
    {
        try
        {
            stateMachine.SetFloat(name, value);
        }
        catch (Exception) { }
    }

    public float GetFloat(string name)
    {
        return stateMachine.GetFloat(name);
    }

    public void SetInt(string name, int value)
    {
        try
        {
            stateMachine.SetInt(name, value);
        }
        catch (Exception) { }
    }
    public int GetInt(string name)
    {
        return stateMachine.GetInt(name);
    }
    public void SetBool(string name, bool value)
    {
        try
        {
            stateMachine.SetBool(name, value);
        }
        catch (Exception) { }
    }
    public bool GetBool(string name)
    {
        return stateMachine.GetBool(name);
    }
    public void SetTrigger(string name)
    {
        try
        {
            stateMachine.SetTrigger(name);
        }
        catch (Exception) { }
    }

    public FSMCState GetCurrentState()
    {
        return stateMachine.GetCurrentState();
    }
    public void SetCurrentState(string name)
    {
        stateMachine.SetCurrentState(name, this);
    }
    public FSMCState GetState(string name)
    {
        return stateMachine.GetState(name);
    }
}
