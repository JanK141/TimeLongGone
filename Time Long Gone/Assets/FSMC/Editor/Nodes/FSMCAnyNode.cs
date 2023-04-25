using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace FSMC.Nodes
{
    public class FSMCAnyNode : FSMCBaseNode
    {
        public FSMCAnyNode(Vector2 pos) : base("Any", pos)
        {
            
        }

        public override void Draw()
        {
            base.Draw();

            Port port = Port.Create<FSMCEdge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            port.AddToClassList("InvisPort");
            Insert(0, port);

            AddContextTransition(port, IndexOf(port), "Create Transition");
        }

    }
}
