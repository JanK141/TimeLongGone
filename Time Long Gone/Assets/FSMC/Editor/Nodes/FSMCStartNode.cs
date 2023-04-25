using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.Port;
using static UnityEditor.Graphs.EdgeGUI;

namespace FSMC.Nodes
{
    public class FSMCStartNode : FSMCBaseNode
    {

        public FSMCStartNode(Vector2 pos) : base("Start", pos)
        {
            
        }

        public override void Draw()
        {
            base.Draw();

            Port port = Port.Create<FSMCEdge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            port.AddToClassList("InvisPort");
            Insert(0, port);

            AddContextTransition(port, IndexOf(port), "Create Transition");

            /*this.AddManipulator(new ContextualMenuManipulator(
                menuEvent => {
                    menuEvent.menu.AppendAction("Create Transition",
                    (e) =>
                    {
                        RemoveAt(0);
                        Insert(childCount, port);
                        port.SendEvent(new SimulatePress(port.GetGlobalCenter()));
                        RemoveAt(childCount - 1);
                        Insert(0, port);
                    }, DropdownMenuAction.AlwaysEnabled);
                }));*/
        }

    }
}


