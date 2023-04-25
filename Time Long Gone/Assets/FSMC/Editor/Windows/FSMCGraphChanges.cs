using FSMC.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace FSMC.Windows
{
    public partial class FSMCGraphView
    {
        private GraphViewChange OnGraphViewChange(GraphViewChange change)
        {
            Undo.SetCurrentGroupName("State Machine changed");
            int group = Undo.GetCurrentGroup();
            Undo.RegisterCompleteObjectUndo(Controller, "State Machine changed");

            //EDGES CREATION

            if (change.edgesToCreate != null)
            {
                foreach (Edge edge in change.edgesToCreate)
                {
                    //CREATING START EDGE
                    if (edge.output.node is FSMCStartNode)
                    {
                        Controller.StartingState = (edge.input.node as FSMCStateNode).State;
                        if (this.Query<Edge>().Where(e => e.output.node is FSMCStartNode && !e.Equals(edge)).ToList().Count() > 0)
                        {
                            this.RemoveElement(this.Query<Edge>().Where(e => e.output.node is FSMCStartNode && !e.Equals(edge)).First());
                        }
                    }
                    //CREATING ANY EDGE
                    else if (edge.output.node is FSMCAnyNode)
                    {
                        var transition = ScriptableObject.CreateInstance<FSMCTransition>();
                        Undo.RegisterCreatedObjectUndo(transition, "Create Transition");
                        transition.name = "Any->" + (edge.input.node as FSMCBaseNode).NodeName;
                        transition.DestinationState = (edge.input.node as FSMCStateNode).State;
                        transition.StateMachine = Controller;

                        var wrap = new FSMCConditionWrapper();
                        wrap.conditions = new();
                        transition.conditions = new() { wrap };

                        AssetDatabase.AddObjectToAsset(transition, Controller);
                        Controller.AnyTransitions.Add(transition);
                        Undo.RegisterCompleteObjectUndo((edge.input.node as FSMCStateNode).State, "Create Transition");
                        (edge.input.node as FSMCStateNode).State.TransitionsTo.Add(transition);
                        (edge as FSMCEdge).transition = transition;
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    //CREATING EDGE BETWEEN STATES
                    else
                    {
                        var transition = ScriptableObject.CreateInstance<FSMCTransition>();
                        Undo.RegisterCreatedObjectUndo(transition, "Create Transition");
                        transition.name = (edge.output.node as FSMCBaseNode).NodeName + "->" + (edge.input.node as FSMCBaseNode).NodeName;
                        transition.DestinationState = (edge.input.node as FSMCStateNode).State;
                        transition.OriginState = (edge.output.node as FSMCStateNode).State;
                        transition.StateMachine = Controller;

                        var wrap = new FSMCConditionWrapper();
                        wrap.conditions = new();
                        transition.conditions = new() { wrap };

                        AssetDatabase.AddObjectToAsset(transition, Controller);
                        Undo.RegisterCompleteObjectUndo((edge.input.node as FSMCStateNode).State, "Create Transition");
                        Undo.RegisterCompleteObjectUndo((edge.output.node as FSMCStateNode).State, "Create Transition");
                        (edge.output.node as FSMCStateNode).State.TransitionsFrom.Add(transition);
                        (edge.input.node as FSMCStateNode).State.TransitionsTo.Add(transition);
                        (edge as FSMCEdge).transition = transition;
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }

            //MOVING NODES


            if (change.movedElements != null)
            {
                foreach (var element in change.movedElements)
                {
                    if (element is FSMCStartNode)
                    {
                        Controller.StartPosition = new Vector2((element as Node).GetPosition().x, (element as Node).GetPosition().y);
                    }
                    else if (element is FSMCAnyNode)
                    {
                        Controller.AnyPosition = new Vector2((element as Node).GetPosition().x, (element as Node).GetPosition().y);
                    }
                    else if (element is FSMCStateNode)
                    {
                        FSMCStateNode node = element as FSMCStateNode;
                        Undo.RegisterCompleteObjectUndo(node.State, "State Machine changed");
                        node.State.Position = new Vector2(node.GetPosition().x, node.GetPosition().y);
                    }
                }
            }

            //REMOVING ELEMENTS

            if (change.elementsToRemove != null)
            {
                //SAFE CHECK & TRANSITIONS CASCADE
                for (int i = change.elementsToRemove.Count - 1; i >= 0; i--)
                {
                    //ADD ALL TRANSITIONS TO REMOVAL ON STATE DELETION
                    if (change.elementsToRemove[i] is FSMCStateNode)
                    {
                        var node = change.elementsToRemove[i] as FSMCStateNode;
                        node.Query<Port>().ForEach(p => change.elementsToRemove.AddRange(p.connections));
                    }
                    //PREVENT DELETING START AND ANY NODE
                    else if (change.elementsToRemove[i] is FSMCAnyNode || change.elementsToRemove[i] is FSMCStartNode)
                    {
                        change.elementsToRemove.RemoveAt(i);
                    }
                    //PREVENT DELETING START TRANSITION
                    else if (change.elementsToRemove[i] is FSMCEdge)
                    {
                        var edge = change.elementsToRemove[i] as FSMCEdge;
                        if (edge.transition == null) //That means it is a start transition
                        {
                            change.elementsToRemove.RemoveAt(i);
                        }
                    }
                }

                //DISTINCT FOR SAFETY. USER COULD SELECT MULTIPLE ELEMENTS TO DELETE AT ONCE, SOME OF THEM COULD BE ADDED SECOND TIME IN PREVIOUS OPERATIONS
                change.elementsToRemove = change.elementsToRemove.Distinct().ToList();

                foreach (var e in change.elementsToRemove)
                {
                    if (e is FSMCStateNode)
                    {
                        Controller.States.Remove((e as FSMCStateNode).State);
                        Undo.DestroyObjectImmediate((e as FSMCStateNode).State);
                    }
                    else if (e is FSMCEdge)
                    {
                        var edge = e as FSMCEdge;
                        if (edge.transition == null) //That means it is a start transition
                        {
                            Controller.StartingState = null;
                            this.Query<FSMCStateNode>().Where(n => !change.elementsToRemove.Contains(n)).First()?.SetAsStart(Controller);
                        }
                        else if (edge.transition != null && edge.transition.OriginState == null) //That means it is an any transition
                        {
                            Controller.AnyTransitions.Remove(edge.transition);
                            Undo.RegisterCompleteObjectUndo(edge.transition.DestinationState, "State Machine changed");
                            edge.transition.DestinationState.TransitionsTo.Remove(edge.transition);
                            Undo.DestroyObjectImmediate(edge.transition);
                        }
                        else if(edge.transition != null) //Simple transition
                        {
                            Undo.RegisterCompleteObjectUndo(edge.transition.OriginState, "State Machine changed");
                            Undo.RegisterCompleteObjectUndo(edge.transition.DestinationState, "State Machine changed");
                            edge.transition.OriginState.TransitionsFrom.Remove(edge.transition);
                            edge.transition.DestinationState.TransitionsTo.Remove(edge.transition);
                            Undo.DestroyObjectImmediate(edge.transition);
                        }
                    }
                }
            }

            Undo.CollapseUndoOperations(group);
            //AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return change;
        }
    }
}
