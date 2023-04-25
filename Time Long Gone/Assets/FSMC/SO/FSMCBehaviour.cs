using FSMC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMCBehaviour : ScriptableObject
{
    public bool enabled = true;

    public virtual void StateInit(FSMController stateMachine, FSMCExecuter executer)
    {

    }
    public virtual void OnStateEnter(FSMController stateMachine, FSMCExecuter executer)
    {

    }

    public virtual void OnStateUpdate(FSMController stateMachine, FSMCExecuter executer)
    {

    }

    public virtual void OnStateExit(FSMController stateMachine, FSMCExecuter executer)
    {

    }
}
