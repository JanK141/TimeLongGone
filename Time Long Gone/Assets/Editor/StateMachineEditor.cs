using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(StateMachine))]
public class StateMachineEditor : UnityEditor.Editor
{
    private SerializedObject so;
    private int selectedState;

    private bool showParameters = true;
    private bool showStates = true;

    private ReorderableList parametersList;
    private ReorderableList statesList;

    void OnEnable()
    {
        so = serializedObject;
        parametersList = new ReorderableList(so, so.FindProperty("parameters"), true, true, true, true);
        parametersList.drawElementCallback = DrawParameter;
        parametersList.drawHeaderCallback = DrawParametersHeader;
        parametersList.onAddDropdownCallback = AddParameterDropdown;
        parametersList.onRemoveCallback = RemoveParameterInList;

        statesList = new ReorderableList(so, so.FindProperty("states"), true, true, true, true);
        statesList.drawElementCallback = DrawState;
        statesList.drawHeaderCallback = DrawStatesHeader;
        statesList.onAddCallback = AddStateInList;
        statesList.onRemoveCallback = RemoveStateInList;
    }

    void DrawParameter(Rect rect, int index, bool isActive, bool isFocused)
    {
        int currWidth = Screen.width-10;
        SerializedObject element = new SerializedObject(parametersList.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue);
        element.Update();
        string label = (element.targetObject is FloatParameter) ? "Float" :
            (element.targetObject is BoolParameter) ? "Bool" : "Int";
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 35, EditorGUIUtility.singleLineHeight), label);
        EditorGUI.PropertyField(new Rect(rect.x + 40, rect.y, (int)(0.5*currWidth), EditorGUIUtility.singleLineHeight),
            element.FindProperty("paramName"), new GUIContent());
        EditorGUI.LabelField(new Rect(rect.x + 40 + (int)(0.6 * currWidth), rect.y, (int)(0.2 * currWidth), EditorGUIUtility.singleLineHeight), "Value");
        EditorGUI.PropertyField(new Rect(rect.x + 40 + (int)(0.7 * currWidth), rect.y, (int)(0.1 * currWidth), EditorGUIUtility.singleLineHeight),
            element.FindProperty("value"), new GUIContent());
        if (element.ApplyModifiedProperties())
        {
            if (element.targetObject.name != (element.targetObject as Parameter).paramName)
            {
                element.targetObject.name = "("+label+")"+(element.targetObject as Parameter).paramName;
                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(element.targetObject);
            }
        }
    }
    void DrawParametersHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "State Machine Parameters");
    }
    void AddParameterDropdown(Rect rect, ReorderableList l)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add Float"), false, AddFloat);
        menu.AddItem(new GUIContent("Add Int"), false, AddInt);
        menu.AddItem(new GUIContent("Add Bool"), false, AddBool);
        menu.ShowAsContext();
    }
    void RemoveParameterInList(ReorderableList l) => DeleteParam(l.index);

    void DrawState(Rect rect, int index, bool isActive, bool isFocused)
    {
        int currWidth = Screen.width - 10;
        SerializedObject element = new SerializedObject(statesList.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue);
        element.Update();
        EditorGUI.PropertyField(new Rect(rect.x, rect.y, (int)(0.45*currWidth), EditorGUIUtility.singleLineHeight),
            element.FindProperty("stateName"), new GUIContent());
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUI.PropertyField(
                new Rect(rect.x + (int) (0.5 * currWidth), rect.y, (int) (0.4 * currWidth),
                    EditorGUIUtility.singleLineHeight), statesList.serializedProperty.GetArrayElementAtIndex(index),
                new GUIContent());
        }
        if (element.ApplyModifiedProperties())
        {
            if (element.targetObject.name != (element.targetObject as SMState).stateName)
            {
                element.targetObject.name = (element.targetObject as SMState).stateName + "(State)";
                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(element.targetObject);
            }
        }
    }
    void DrawStatesHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "All states (double click reference field to open state)");
    }
    void AddStateInList(ReorderableList l) => AddState();
    void RemoveStateInList(ReorderableList l) => DeleteState(l.index);

    public override void OnInspectorGUI()
    {
        so.Update();
        showParameters = EditorGUILayout.Foldout(showParameters, "Parameters");
        if(showParameters) parametersList.DoLayoutList();

        showStates = EditorGUILayout.Foldout(showStates, "States");
        if(showStates)statesList.DoLayoutList();
        
        
        GUILayout.Space(15);

        string[] options = (so.targetObject as StateMachine).states.Select(x => x.stateName).ToArray();
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label("Initial state:");
            if ((so.targetObject as StateMachine).initialState != null)
            {
                var target = so.targetObject as StateMachine;
                selectedState = target.states.IndexOf(target.initialState);
            }else if (options.Length > 0)
            {
                (so.targetObject as StateMachine).initialState =
                    (so.targetObject as StateMachine).states[selectedState];
            }
            int selection = EditorGUILayout.Popup(selectedState,
                (options.Length > 0) ? options : new[] {""}, GUILayout.MinWidth(50));
            if (selection != selectedState && options.Length != 0)
            {
                Undo.IncrementCurrentGroup();
                Undo.SetCurrentGroupName("set initial state");
                Undo.RegisterCompleteObjectUndo(this as Object, "");
                selectedState = selection;
                Undo.RegisterCompleteObjectUndo(so.targetObject as StateMachine, "");
                (so.targetObject as StateMachine).initialState =
                    (so.targetObject as StateMachine).states[selectedState];
                Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            }
        }

        so.ApplyModifiedProperties();
    }

    void AddState()
    {
        var target = so.targetObject as StateMachine;
        SMState newState = ScriptableObject.CreateInstance<SMState>();
        newState.name = "New State";
        newState.Init(target, "New State");

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("add state");
        AssetDatabase.AddObjectToAsset(newState, target);
        AssetDatabase.SaveAssets();
        Undo.RegisterCreatedObjectUndo(newState, "");
        Undo.RegisterCompleteObjectUndo(target, "");
        target.states.Add(newState);
        EditorUtility.SetDirty(target);
        EditorUtility.SetDirty(newState);

        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
    }

    void DeleteState(int index)
    {
        var spar = new SerializedObject(statesList.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue);
        var target = spar.targetObject as SMState;
        List<SMTransition> transitionsFrom = target.transitions;
        List<SMTransition> transitionsTo =
            target.parent.states.SelectMany(s => s.transitions).Where(t => t.to == target).ToList();

        if (transitionsFrom.Count > 0 || transitionsTo.Count > 0)
        {
            if (!EditorUtility.DisplayDialog("Deleting state",
                    $"Are you sure you want to delete state with {transitionsFrom.Count} transitions from, and {transitionsTo.Count} transitions to it?",
                    "Yes", "No")) return;
        }

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("delete state");
        var groupIndex = Undo.GetCurrentGroup();
        Undo.RegisterCompleteObjectUndo(so.targetObject, "");
        Undo.RegisterCompleteObjectUndo(spar.targetObject, "");
        foreach (SMTransition transition in transitionsFrom)
        {
            Undo.DestroyObjectImmediate(transition);
        }

        foreach (SMTransition transition in transitionsTo)
        {
            Undo.RegisterCompleteObjectUndo(transition.@from, "");
            transition.@from.transitions.Remove(transition);
            Undo.DestroyObjectImmediate(transition);
        }
        ((SMState)spar.targetObject).parent.states.Remove((SMState)spar.targetObject);
        if (index == selectedState) ((SMState) spar.targetObject).parent.initialState = null;
        if (index == selectedState) selectedState = 0;

        Undo.DestroyObjectImmediate(spar.targetObject);
        AssetDatabase.SaveAssets();
        Undo.CollapseUndoOperations(groupIndex);
    }

    void DeleteParam(int index)
    {
        List<SMCondition> conditions = (so.targetObject as StateMachine).states.SelectMany(s => s.transitions)
            .SelectMany(t => t.conditions).Where(c => c.parameter == (so.targetObject as StateMachine).parameters[index]).ToList();

        if (conditions.Count > 0)
        {
            if (!EditorUtility.DisplayDialog("Deleting parameter",
                    $"Are you sure you want to delete parameter and {conditions.Count} conditions associated with it?",
                    "Yes", "No")) return;
        }

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("delete parameter");
        foreach (SMCondition condition in conditions)
        {
            Undo.RegisterCompleteObjectUndo(condition.parent, "");
            if (condition.parent.conditions.Count == 1)
            {
                Undo.RegisterCompleteObjectUndo(condition.parent.@from, "");
                condition.parent.@from.transitions.Remove(condition.parent);
                Undo.DestroyObjectImmediate(condition.parent);
            }
            else
            {
                condition.parent.conditions.Remove(condition);
            }
        }
        var groupIndex = Undo.GetCurrentGroup();
        var spar = new SerializedObject(parametersList.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue);
        Undo.RecordObject(so.targetObject, "");
        ((Parameter) spar.targetObject).parent.parameters.Remove((Parameter) spar.targetObject);
        Undo.DestroyObjectImmediate(spar.targetObject);
        AssetDatabase.SaveAssets();
        Undo.CollapseUndoOperations(groupIndex);
    }

    void AddFloat()
    {
        var target = so.targetObject as StateMachine;
        FloatParameter floatParameter = ScriptableObject.CreateInstance<FloatParameter>();
        floatParameter.name = "Float";
        floatParameter.Init("Float", new float(), target);
        SaveParam(floatParameter, target);
    }

    void AddInt()
    {
        var target = so.targetObject as StateMachine;
        IntParameter intParameter = ScriptableObject.CreateInstance<IntParameter>();
        intParameter.name = "Int";
        intParameter.Init("Int", new int(), target);
        SaveParam(intParameter, target);
    }

    void AddBool()
    {
        var target = so.targetObject as StateMachine;
        BoolParameter boolParameter = ScriptableObject.CreateInstance<BoolParameter>();
        boolParameter.name = "Bool";
        boolParameter.Init("Bool", new bool(), target);
        SaveParam(boolParameter, target);
    }

    void SaveParam(Parameter param, StateMachine target)
    {
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("add parameter");
        AssetDatabase.AddObjectToAsset(param, target);
        AssetDatabase.SaveAssets();
        Undo.RegisterCreatedObjectUndo(param, "");
        Undo.RegisterCompleteObjectUndo(target, "");
        target.parameters.Add(param);
        EditorUtility.SetDirty(target);
        EditorUtility.SetDirty(param);
        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
    }
}
