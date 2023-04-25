using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace FSMC
{

    [CreateAssetMenu(fileName = "FSM_controller", menuName = "FSMC/Controller")]
    public class FSMController : ScriptableObject
    {
#if UNITY_EDITOR
        public Vector2 StartPosition = new Vector2(200,200);
        public Vector2 AnyPosition = new Vector2(200,400);
#endif
        public List<FSMCState> States = new List<FSMCState>();
        public List<FSMCTransition> AnyTransitions = new List<FSMCTransition>();
        public FSMCState StartingState;
        public List<FSMParameter> Parameters = new List<FSMParameter>();


        private FSMCState _currentState;
        private FSMCState _transitioningTo;
        private List<FSMBoolParameter> _triggersActive;

        public void StartStateMachine(FSMCExecuter executer)
        {
            _triggersActive = new();
            foreach(var state in States)
            {
                state.StateInit(this, executer);
            }
            _transitioningTo = StartingState;
        }

        public void UpdateStateMachine(FSMCExecuter executer)
        {
            if(_transitioningTo != null)
            {
                _currentState = _transitioningTo;
                _transitioningTo = null;
                _currentState.OnStateEnter(this, executer);
            }

            _currentState.OnStateUpdate(this, executer);

            FSMCState state = null;
            foreach(var transition in AnyTransitions)
            {
                state = transition.Evaluate();
                if (state != null) break;
            }

            if (state == null)
                state = _currentState.Evaluate();

            if(state != null)
            {
                _currentState.OnStateExit(this, executer);
                _transitioningTo = state;
            }

            for(int i = _triggersActive.Count - 1; i >= 0; i--)
            {
                _triggersActive[i].Value = false;
                _triggersActive.RemoveAt(i);
            }
        }




        public void SetFloat(string name, float value)
        {
            (Parameters.SingleOrDefault(p => p.name == name && p.Type == FSMParameterType.Float) as FSMFloatParameter).Value = value;
        }

        public float GetFloat(string name)
        {
            return (Parameters.SingleOrDefault(p => p.name == name && p.Type == FSMParameterType.Float) as FSMFloatParameter).Value;
        }

        public void SetInt(string name, int value)
        {
            (Parameters.SingleOrDefault(p => p.name == name && p.Type == FSMParameterType.Integer) as FSMIntegerParameter).Value = value;
        }
        public int GetInt(string name)
        {
            return (Parameters.SingleOrDefault(p => p.name == name && p.Type == FSMParameterType.Integer) as FSMIntegerParameter).Value;
        }
        public void SetBool(string name, bool value)
        {
            (Parameters.SingleOrDefault(p => p.name == name && p.Type == FSMParameterType.Bool) as FSMBoolParameter).Value = value;
        }
        public bool GetBool(string name)
        {
            return (Parameters.SingleOrDefault(p => p.name == name && p.Type == FSMParameterType.Bool) as FSMBoolParameter).Value;
        }
        public void SetTrigger(string name)
        {
            FSMBoolParameter trigger = (Parameters.SingleOrDefault(p => p.name == name && p.Type == FSMParameterType.Trigger) as FSMBoolParameter);
            trigger.Value = true;
            _triggersActive.Add(trigger);
        }

        public FSMCState GetCurrentState()
        {
            return _currentState;
        }
        public void SetCurrentState(string name, FSMCExecuter executer)
        {
            FSMCState state = States.SingleOrDefault(s => s.name == name);
            if(state != null)
            {
                _currentState.OnStateExit(this, executer);
                _transitioningTo = state;
            }
        }
        public FSMCState GetState(string name)
        {
            return States.SingleOrDefault(s => s.name == name);
        }

    }
}
