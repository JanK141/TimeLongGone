using FSMC;
using FSMC.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(FSMCState))]
public class FSMCStateEditor : UnityEditor.Editor
{
    private SerializedProperty _behavioursProperty;
    private SerializedProperty _transitionsFrom;
    private SerializedProperty _transitionsTo;

    VisualElement root;
    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        root.Add(GenerateContent());
        return root;
    }
    private VisualElement GenerateContent()
    {
        var root = new VisualElement();

        _behavioursProperty = serializedObject.FindProperty("_behaviours");
        _transitionsFrom = serializedObject.FindProperty("TransitionsFrom");
        _transitionsTo = serializedObject.FindProperty("TransitionsTo");

        for (int i = 0; i < _transitionsFrom.arraySize; i++)
        {
            var p = new PropertyField(_transitionsFrom.GetArrayElementAtIndex(i), "");
            root.Add(p);
            p.BindProperty(_transitionsFrom.GetArrayElementAtIndex(i));
            p.AddToClassList("transition");
        }
        for (int i = 0; i < _transitionsTo.arraySize; i++)
        {
            var p = new PropertyField(_transitionsTo.GetArrayElementAtIndex(i), "");
            root.Add(p);
            p.BindProperty(_transitionsTo.GetArrayElementAtIndex(i));
            p.AddToClassList("transition");
        }

        // Create a VisualElement for each behaviour
        for (int i = 0; i < _behavioursProperty.arraySize; i++)
        {
            var behaviour = new SerializedObject(_behavioursProperty.GetArrayElementAtIndex(i).objectReferenceValue);
            var childProperty = behaviour.GetIterator();

            var foldout = new Foldout();
            var foldSpace = foldout.Q(className: "unity-base-field__input");
            foldSpace.name = "BehaviourFold";
            childProperty.NextVisible(true);
            childProperty.NextVisible(true);
            var enabled = new PropertyField(childProperty, "");
            foldSpace.Add(enabled);
            enabled.BindProperty(childProperty);
            foldSpace.Add(new Image() { image = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D });
            foldSpace.Add(new Label() { text = behaviour.targetObject.GetType().Name });
            int j = i;
            var opt = new Button(() => ShowMenuForBehaviour(j)) { name = "OptionsButton" };
            var optContainer = new VisualElement() { name = "OptionsContainer" };
            optContainer.Add(opt);
            foldSpace.Add(optContainer);

            while (childProperty.NextVisible(false))
            {
                var p = new PropertyField(childProperty);
                foldout.Add(p);
                p.BindProperty(childProperty);
            }

            root.Add(foldout);
        }


        // Create a button to add new behaviours
        var dropdownMenu = new GenericDropdownMenu();
        var types = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(FSMCBehaviour).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface);

        foreach (var type in types)
        {
            dropdownMenu.AddItem(type.Name, false, () => AddBehaviourOfType(type));
        }

        var addButton = new Button(()=>dropdownMenu.DropDown(root.worldBound, root, true)) { text = "Add Behaviour" };
        root.Add(addButton);

        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Assets/FSMC/Editor/Editor Resources/FSMCStateInspectorStyles.uss");
        root.styleSheets.Add(styleSheet);

        return root;
    }

    private void ShowMenuForBehaviour(int index)
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Delete"), false, () => RemoveBehaviour(index));
        menu.ShowAsContext();
    }
    private void RemoveBehaviour(int index)
    {
        var behaviour = _behavioursProperty.GetArrayElementAtIndex(index).objectReferenceValue as FSMCBehaviour;
        _behavioursProperty.DeleteArrayElementAtIndex(index);
        ScriptableObject.DestroyImmediate(behaviour, true);
        serializedObject.ApplyModifiedProperties();
        root.Clear();
        root.Add(GenerateContent());
    }

    private void AddBehaviourOfType(System.Type type)
    {
        var behaviour = ScriptableObject.CreateInstance(type) as FSMCBehaviour;
        behaviour.hideFlags = HideFlags.HideInHierarchy;

        AssetDatabase.AddObjectToAsset(behaviour, serializedObject.targetObject);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(behaviour));

        _behavioursProperty.arraySize++;
        _behavioursProperty.GetArrayElementAtIndex(_behavioursProperty.arraySize - 1).objectReferenceValue = behaviour;

        serializedObject.ApplyModifiedProperties();
        root.Clear();
        root.Add(GenerateContent());
    }

}

