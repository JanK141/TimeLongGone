using FSMC.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace FSMC.Nodes
{
    public class FSMCStateNode : FSMCBaseNode
    {
        public FSMCState State;
        private Port inputPort;
        private Port outputPort;

        public FSMCStateNode(FSMCState state) : base(state.name, state.Position)
        {
            State = state;
        }
        public FSMCStateNode(string stateName, FSMController controller, Vector2 pos) : base(stateName, pos)
        {
            State = ScriptableObject.CreateInstance<FSMCState>();
            State.name = stateName;
            AssetDatabase.AddObjectToAsset(State, controller);

            Undo.SetCurrentGroupName("Create state");
            int group = Undo.GetCurrentGroup();
            Undo.RegisterCreatedObjectUndo(State, "Create state");
            Undo.RegisterCompleteObjectUndo(controller, "Create state");
            Undo.CollapseUndoOperations(group);

            controller.States.Add(State);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            State.Position = pos;
        }

        public void SetAsStart(FSMController controller)
        {
            controller.StartingState = State;
            var graph = GetFirstAncestorOfType<FSMCGraphView>();
            graph.AddElement(graph.Q<FSMCStartNode>().Q<Port>(className: "output").ConnectTo<FSMCEdge>(inputPort));
        }

        public override void Draw()
        {
            base.Draw();

            outputPort = Port.Create<FSMCEdge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            outputPort.AddToClassList("InvisPort");
            inputPort = Port.Create<FSMCEdge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.AddToClassList("InvisPort");
            Insert(0,outputPort);
            Insert(0,inputPort);
            AddContextTransition(outputPort, IndexOf(outputPort), "Create Transition");
        }

        public override void OnSelected()
        {
            base.OnSelected();
            AssetDatabase.OpenAsset(State.GetInstanceID());
        }

    }
}
