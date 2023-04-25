using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace FSMC.Nodes
{
    public abstract class FSMCBaseNode : Node
    {
        public string NodeName { get; set; }

        public FSMCBaseNode(string nodeName, Vector2 pos)
        {
            NodeName = nodeName;
            Initialize(pos);
        }

        public virtual void Initialize(Vector2 pos)
        {
            SetPosition(new Rect(pos, Vector2.zero));

            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Assets/FSMC/Editor/Editor Resources/FSMCNodeStyles.uss");
            styleSheets.Add(styleSheet);

            Draw();
        }
        public virtual void Draw()
        {
            mainContainer.Clear();
            mainContainer.name = "main";

            Label label = new Label(NodeName);
            label.StretchToParentSize();

            label.AddToClassList("MainTitleClass");
            mainContainer.Add(label);
            
        }

        protected void AddContextTransition(Port port, int index, string menuName)
        {
            this.AddManipulator(new ContextualMenuManipulator(
                menuEvent => {
                    menuEvent.menu.AppendAction(menuName,
                    (e) =>
                    {
                        RemoveAt(index);
                        Insert(childCount, port);
                        port.SendEvent(new SimulatePress(port.GetGlobalCenter()));
                        RemoveAt(childCount - 1);
                        Insert(index, port);
                    }, DropdownMenuAction.AlwaysEnabled);
                }));
        }

        protected class SimulatePress : MouseDownEvent
        {
            public SimulatePress(Vector3 position)
            {
                mousePosition = new Vector2(position.x, position.y);
                button = 0;
            }
        }
    }
}
