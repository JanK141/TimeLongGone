using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

namespace FSMC.Windows
{
    using Nodes;
    using System.Linq;

    public partial class FSMCGraphView : GraphView
    {
        public FSMController Controller { get; private set; }
        public FSMCStartNode Start { get; private set; }

        public FSMCGraphView(FSMController controller)
        {
            Controller = controller;

            AddManipulators();

            AddBackgound();

            InitializeNodes();

            AddStyles();

            graphViewChanged += OnGraphViewChange;

            /*Undo.undoRedoPerformed += () => {
                //this.Clear();
                //this.Remo(this.Query<GraphElement>().Where(e => e is Node || e is Edge).ToList());
                for(int i =1; i<this.childCount; i++)
                {
                    this.RemoveAt(i);
                }
                //AddManipulators();
                //AddBackgound();
                InitializeNodes();
                //AddStyles();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            };*/
        }

        private void InitializeNodes()
        {
            Start = new FSMCStartNode(Controller.StartPosition);
            var any = new FSMCAnyNode(Controller.AnyPosition);
            AddElement(Start);
            AddElement(any);

            List<FSMCStateNode> states = new List<FSMCStateNode>();

            foreach(FSMCState state in Controller.States)
            {
                var node = new FSMCStateNode(state);
                states.Add(node);
                AddElement(node);
            }

            foreach(FSMCTransition transition in Controller.AnyTransitions)
            {
                var edge = any.Q<Port>(className: "output").ConnectTo<FSMCEdge>(states.Single(s => s.NodeName == transition.DestinationState.name).Q<Port>(className: "input"));
                edge.transition = transition;
                AddElement(edge);
            }
            foreach(var state in states)
            {
                foreach(var transition in state.State.TransitionsTo)
                {
                    if(transition.OriginState == null && transition.name.StartsWith("Any->"))
                    {
                        var edge = any.Q<Port>(className: "output").ConnectTo<FSMCEdge>(state.Q<Port>(className: "input"));
                        edge.transition = transition;
                        AddElement(edge);
                    }
                    else
                    {
                        var edge = states.Single(s => s.NodeName == transition.OriginState.name).Q<Port>(className: "output").ConnectTo<FSMCEdge>(state.Q<Port>(className: "input"));
                        edge.transition = transition;
                        AddElement(edge);
                    }
                }
            }
            if (Controller.StartingState != null)
            {
                AddElement(Start.Q<Port>(className: "output").ConnectTo<FSMCEdge>(states.Single(s=>s.NodeName==Controller.StartingState.name).Q<Port>(className:"input")));
            }
        }
        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(CreateContextMenu());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContentDragger());
        }
        private void AddBackgound()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }
        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Assets/FSMC/Editor/Editor Resources/FSMCGraphViewStyles.uss");
            styleSheets.Add(styleSheet);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port) return;
                if (startPort.node == port.node) return;
                if (startPort.direction == port.direction) return;
                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private IManipulator CreateContextMenu()
        {
            ContextualMenuManipulator menu = new(
                menuEvent => menuEvent.menu.AppendAction(
                    "Create State", actionEvent => {
                        var window = new CreateStatePopup(s => CreateNode(contentViewContainer.WorldToLocal(actionEvent.eventInfo.mousePosition), s));
                        UnityEditor.PopupWindow.Show( new Rect(actionEvent.eventInfo.mousePosition, Vector2.zero), window);
                        window.editorWindow.rootVisualElement.Q<TextField>().Q<VisualElement>(name: "unity-text-input").Focus();
                    })
                );

            return menu;
        }

        private void CreateNode(Vector2 pos, string name)
        {
            var node = new FSMCStateNode(name, Controller, pos);
            AddElement(node);
            if (Controller.States.Count == 1)
                node.SetAsStart(Controller);
        }

    }
}
