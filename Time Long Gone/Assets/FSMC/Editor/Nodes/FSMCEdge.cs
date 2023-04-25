using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;
using Image = UnityEngine.UIElements.Image;

namespace FSMC.Nodes
{
    public class FSMCEdge : Edge
    {
        public FSMCTransition transition;

        private FSMCEdgeArrow arrow;

        public FSMCEdge() : base()
        {
            arrow = new(this);
            Add(arrow);
            edgeControl.RegisterCallback<GeometryChangedEvent>(OnEdgeControlGeometryChanged);
        }

        private void OnEdgeControlGeometryChanged(GeometryChangedEvent evt)
        {
            PointsAndTangents[1] = PointsAndTangents[0];
            PointsAndTangents[2] = PointsAndTangents[3];

            if(input!=null && output!=null)
            {
                if (input.node.GetPosition().y > output.node.GetPosition().y)// UP
                {
                    PointsAndTangents[1].x += 8;
                    PointsAndTangents[2].x += 8;
                }
                else if (input.node.GetPosition().y < output.node.GetPosition().y)// Down
                {
                    PointsAndTangents[1].x -= 8;
                    PointsAndTangents[2].x -= 8;
                }
                else if (input.node.GetPosition().y == output.node.GetPosition().y) // Fix a wierd af bug in equally absurd way
                {
                    PointsAndTangents[1].x -= 1;
                    PointsAndTangents[1].y -= 1;
                }

                if (input.node.GetPosition().x > output.node.GetPosition().x)// Right
                {
                    PointsAndTangents[1].y -= 8;
                    PointsAndTangents[2].y -= 8;
                }
                else if(input.node.GetPosition().x < output.node.GetPosition().x)// Left
                {
                    PointsAndTangents[1].y += 8;
                    PointsAndTangents[2].y += 8;
                }
            }
    
            arrow.MarkDirtyRepaint();
        }
        public override void OnSelected()
        {
            base.OnSelected();
            arrow.MarkDirtyRepaint();
            if(transition!=null)AssetDatabase.OpenAsset(transition.GetInstanceID());
        }public override void OnUnselected()
        {
            base.OnUnselected();
            arrow.MarkDirtyRepaint();
        }

        public Vector2[] GetPoints()
        {
            return PointsAndTangents;
        }
    }
}
