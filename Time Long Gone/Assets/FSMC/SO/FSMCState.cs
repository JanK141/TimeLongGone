using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSMC
{
    public class FSMCState : ScriptableObject
    {
        [SerializeField] private List<FSMCBehaviour> _behaviours = new();

#if UNITY_EDITOR
        public Vector2 Position;
#endif
        public List<FSMCTransition> TransitionsFrom = new List<FSMCTransition>();
        public List<FSMCTransition> TransitionsTo = new List<FSMCTransition>();

        public void StateInit(FSMController stateMachine, FSMCExecuter executer)
        {
            foreach (FSMCBehaviour behaviour in _behaviours)
                behaviour.StateInit(stateMachine, executer);
        }
        public void OnStateEnter(FSMController stateMachine, FSMCExecuter executer)
        {
            foreach (FSMCBehaviour behaviour in _behaviours)
                behaviour.OnStateEnter(stateMachine, executer);
        }
        public void OnStateUpdate(FSMController stateMachine, FSMCExecuter executer)
        {
            foreach (FSMCBehaviour behaviour in _behaviours)
                behaviour.OnStateUpdate(stateMachine, executer);
        }
        public void OnStateExit(FSMController stateMachine, FSMCExecuter executer)
        {
            foreach (FSMCBehaviour behaviour in _behaviours)
                behaviour.OnStateExit(stateMachine, executer);
        }
        public FSMCState Evaluate()
        {
            foreach(FSMCTransition transition in TransitionsFrom)
            {
                var state = transition.Evaluate();
                if (state != null) return state;
            }
            return null;
        }
    }
}
