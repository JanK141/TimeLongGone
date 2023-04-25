using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace FSMC.Nodes
{
    public class FSMCEdgeArrow : VisualElement
    {
        private Vector2 start;
        private Vector2 end;
        private FSMCEdge edge;

        public FSMCEdgeArrow(FSMCEdge edge)
        {
            this.edge = edge;
            generateVisualContent += OnGenerateVisualContent;
        }

        private void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
            //Safe check if edge has assigned points
            if (edge.GetPoints().Length < 2) return;

            //Use color same as edge itself
            Color color;
            if (edge.isGhostEdge) color = edge.ghostColor;
            else if (edge.selected) color = edge.selectedColor;
            else color = edge.defaultColor;

            //Assign points beetwen which the edge with an arrow is drawn
            start = edge.GetPoints()[edge.GetPoints().Length/2 - 1];
            end = edge.GetPoints()[edge.GetPoints().Length/2];
            Vector2 lineDirection = end - start;
            Vector2 midPoint = (start + end) / 2;
            float arrowEdgeLength = 12f;
            //Arrow is an equilateral triangle and we want its middle to be in a midPoint, so we shift the points by height/2
            float distanceFromMiddle = (arrowEdgeLength * Mathf.Sqrt(3) / 4);

            //We want bottom edge of a triangle to be always perpendicular to edge it is drawn upon
            //Thus we need to scale it properly
            float angle = Vector2.SignedAngle(Vector2.right, lineDirection);
            float perpendicularLength = arrowEdgeLength / (Mathf.Sin(Mathf.Deg2Rad * (angle - 60)) * 2);

            if (angle < 60 && angle > 0)
            {
                perpendicularLength = arrowEdgeLength / (Mathf.Sin(Mathf.Deg2Rad * (angle + 120)) * 2);
            }
            else if (angle > -120 && angle < -60)
            {
                perpendicularLength = arrowEdgeLength / (Mathf.Sin(Mathf.Deg2Rad * (angle - 120)) * 2);
            }
            else if (angle > -60 && angle < 0)
            {
                perpendicularLength = arrowEdgeLength / (Mathf.Sin(Mathf.Deg2Rad * (angle + 60)) * 2);
            }

            Vector2 perpendicular = new Vector2(-lineDirection.y, lineDirection.x).normalized * perpendicularLength;

            //After all that boring math we assign vertices
            MeshWriteData mesh = ctx.Allocate(3, 3);
            Vertex[] vertices = new Vertex[3];
            vertices[0].position = midPoint + (lineDirection.normalized * distanceFromMiddle);
            vertices[1].position = (midPoint + (-lineDirection.normalized * distanceFromMiddle))+(perpendicular.normalized * arrowEdgeLength/2);
            vertices[2].position = (midPoint + (-lineDirection.normalized * distanceFromMiddle))+(-perpendicular.normalized * arrowEdgeLength/2);

            for(int i = 0; i < vertices.Length; i++)
            {
                vertices[i].position += Vector3.forward * Vertex.nearZ;
                vertices[i].tint = color;
            }

            mesh.SetAllVertices(vertices);
            mesh.SetAllIndices(new ushort[] { 0, 1, 2 });
        }

    }
}
