using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = System.Object;

[CustomEditor(typeof(SMState))]
public class SMStateEditor : UnityEditor.Editor
{
    private SerializedObject so;
    private SerializedProperty parent;
    private SerializedProperty stateName;
    private List<SMState> states;
    private List<SerializedProperty> transitions;

    private ReorderableList transitionsList;

    private bool showTransitions = true;

    void OnEnable()
    {
        so = serializedObject;
        parent = so.FindProperty("parent");
        stateName = so.FindProperty("stateName");

        transitionsList = new ReorderableList(so, so.FindProperty("transitions"), true, true, true, true);
        transitionsList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "All transitions (order matters)");
        transitionsList.onCanAddCallback = list => (so.targetObject as SMState).parent.parameters.Count != 0;
        transitionsList.drawElementCallback = DrawTransition;
        transitionsList.elementHeightCallback = index => (so.targetObject as SMState).transitions[index].conditions.Count * (EditorGUIUtility.singleLineHeight+5);
        transitionsList.onAddDropdownCallback = AddTransitionDropdown;
        transitionsList.onRemoveCallback = list => DeleteTransition(list.index);
    }

    void DrawTransition(Rect rect, int index, bool isActive, bool isFocused)
    {
        int currWidth = Screen.width - 10;
        SerializedObject element =
            new SerializedObject(transitionsList.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue);
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 85, EditorGUIUtility.singleLineHeight),
            element.FindProperty("transitionName").stringValue);
        element.Update();
        var tmpcond = element.FindProperty("conditions");
        List<SerializedProperty> conditions = new List<SerializedProperty>();
        for (int j = 0; j < tmpcond.arraySize; j++)
            conditions.Add(tmpcond.GetArrayElementAtIndex(j));
        using (new GUILayout.AreaScope(new Rect(rect.x + 95, rect.y+EditorGUIUtility.singleLineHeight*5+5,
                       currWidth - 115, tmpcond.arraySize * (EditorGUIUtility.singleLineHeight + 5)), 
                   new GUIContent("Condtiions"), EditorStyles.helpBox))
        {
            for (int j = conditions.Count; j-- > 0;)
            {
                var currY = 2 + (conditions.Count - 1 - j) * (EditorGUIUtility.singleLineHeight + 5);
                var serCon = conditions[j];
                List<Parameter> parameters = (so.targetObject as SMState).parent.parameters;
                int selected =
                    parameters.IndexOf(serCon.FindPropertyRelative("parameter").objectReferenceValue as Parameter);
                int selection = EditorGUI.Popup(
                    new Rect(0, currY, (int) (0.2 * currWidth), EditorGUIUtility.singleLineHeight), selected,
                    parameters.Select(x => x.paramName).ToArray());
                Parameter selectedPar = parameters[selection];
                if (selected != selection)
                {
                    Undo.RecordObject(element.targetObject, "set condition parameter");
                    serCon.FindPropertyRelative("parameter").objectReferenceValue = selectedPar;
                    serCon.FindPropertyRelative("type").enumValueIndex = (selectedPar is IntParameter)
                        ?
                        (int) SMCondition.ParameterType.Int
                        :
                        (selectedPar is FloatParameter)
                            ? (int) SMCondition.ParameterType.Float
                            : (int) SMCondition.ParameterType.Bool;
                }

                EditorGUI.PropertyField(
                    new Rect(5 + (int) (0.2 * currWidth), currY, (int) (0.2 * currWidth),
                        EditorGUIUtility.singleLineHeight),
                    serCon.FindPropertyRelative("comparison"), new GUIContent(""));

                if (selectedPar is IntParameter)
                {
                    EditorGUI.PropertyField(
                        new Rect(10 + (int) (0.4 * currWidth), currY, 50, EditorGUIUtility.singleLineHeight),
                        serCon.FindPropertyRelative("intValue"), new GUIContent(""));
                }
                else if (selectedPar is FloatParameter)
                {
                    EditorGUI.PropertyField(
                        new Rect(10 + (int) (0.4 * currWidth), currY, 50, EditorGUIUtility.singleLineHeight),
                        serCon.FindPropertyRelative("floatValue"), new GUIContent(""));
                }
                else
                {
                    EditorGUI.PropertyField(
                        new Rect(10 + (int) (0.4 * currWidth), currY, 50, EditorGUIUtility.singleLineHeight),
                        serCon.FindPropertyRelative("boolValue"), new GUIContent(""));
                }

                if (GUI.Button(
                        new Rect(15 + (int) (0.55 * currWidth), currY, 20, EditorGUIUtility.singleLineHeight),
                        new GUIContent("-", "Remove condition")))
                    RemoveCondition(element.targetObject as SMTransition, j);
            }

            if (GUI.Button(
                    new Rect(35 + (int) (0.57 * currWidth),
                        2+(tmpcond.arraySize-1) * (EditorGUIUtility.singleLineHeight + 5) / 2, 20,
                        EditorGUIUtility.singleLineHeight),
                    new GUIContent("+", "Add new condition")))
            {
                AddCondition(element.targetObject as SMTransition);
            }
        }

        element.ApplyModifiedProperties();
    }

    void AddTransitionDropdown(Rect rect, ReorderableList l)
    {
        GenericMenu menu = new GenericMenu();
        foreach (SMState state in (so.targetObject as SMState).parent.states)
        {
            menu.AddItem(new GUIContent(stateName.stringValue + " -> " + state.stateName),
                false, AddTransition, state);
        }

        menu.ShowAsContext();
    }

    public override void OnInspectorGUI()
    {
        so.Update();

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.PropertyField(parent, new GUIContent("State Machine"));
        }
        EditorGUILayout.PropertyField(stateName, new GUIContent("State Name"));

        GUILayout.Space(15);

        showTransitions = EditorGUILayout.Foldout(showTransitions, new GUIContent("Transitions"));
        if (showTransitions) transitionsList.DoLayoutList();

        EditorGUILayout.Space(30);

        EditorGUILayout.PropertyField(so.FindProperty("onStateEnter"));
        EditorGUILayout.PropertyField(so.FindProperty("onStateUpdate"));
        EditorGUILayout.PropertyField(so.FindProperty("onStateExit"));

        if (so.ApplyModifiedProperties())
        {
            if ((so.targetObject as SMState).name != stateName.stringValue)
            {
                so.targetObject.name = stateName.stringValue + "(State)";
                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(so.targetObject);
            }
        }
    }

    void RemoveCondition(SMTransition target, int at)
    {
        if(target.conditions.Count==1) return;
        Undo.RecordObject(target, "add condition");
        target.conditions.RemoveAt(at);
    }

    void AddCondition(SMTransition target)
    {
        Undo.RecordObject(target, "add condition");
        var from = so.targetObject as SMState;
        SMCondition condition = new SMCondition();
        var selectedPar = from.parent.parameters[0];
        condition.parameter = selectedPar;
        condition.type = (selectedPar is IntParameter) ? SMCondition.ParameterType.Int :
            (selectedPar is FloatParameter) ? SMCondition.ParameterType.Float : SMCondition.ParameterType.Bool;
        condition.parent = target;
        target.conditions.Add(condition);
    }

    void AddTransition(object state)
    {
        var from = so.targetObject as SMState;
        var to = (SMState) state;
        SMTransition transition = ScriptableObject.CreateInstance<SMTransition>();
        transition.name = from.stateName + "->" + to.stateName+"(Transition)";
        transition.Init(from, to, from.parent);
        SMCondition condition = new SMCondition();
        var selectedPar = from.parent.parameters[0];
        condition.parameter = selectedPar;
        condition.type = (selectedPar is IntParameter) ? SMCondition.ParameterType.Int :
            (selectedPar is FloatParameter) ? SMCondition.ParameterType.Float : SMCondition.ParameterType.Bool;
        condition.parent = transition;
        transition.conditions.Add(condition);

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("add transition");
        AssetDatabase.AddObjectToAsset(transition, from);
        AssetDatabase.SaveAssets();
        Undo.RegisterCreatedObjectUndo(transition, "");
        Undo.RegisterCompleteObjectUndo(target, "");
        from.transitions.Add(transition);
        EditorUtility.SetDirty(from);
        EditorUtility.SetDirty(transition);
        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
    }

    void DeleteTransition(int index)
    {
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("delete transition");
        var groupIndex = Undo.GetCurrentGroup();
        var tran = new SerializedObject(transitionsList.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue);
        Undo.RecordObject(so.targetObject, "");
        (so.targetObject as SMState).transitions.RemoveAt(index);
        Undo.DestroyObjectImmediate(tran.targetObject);
        AssetDatabase.SaveAssets();
        Undo.CollapseUndoOperations(groupIndex);
    }
}
