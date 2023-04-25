using FSMC;
using FSMC.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(FSMCTransition))]
public class FSMCTransitionEditor : UnityEditor.Editor
{
    private List<FSMCConditionWrapper> alternatives;
    VisualElement root;

    private void OnEnable()
    {
        alternatives = (target as FSMCTransition).conditions;
    }

    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        root.Add(GenerateContent());
        return root;
    }

    private VisualElement GenerateContent()
    {
        var root = new VisualElement();
        for (int i = 0; i < alternatives.Count; i++)
        {
            int outerIndex = i;
            var alt = alternatives[outerIndex];
            var conditions = alt.conditions;

            var alternativeElement = new VisualElement();
            alternativeElement.AddToClassList("list-container");
            var listTopBar = new VisualElement();
            listTopBar.AddToClassList("list-top-bar");
            alternativeElement.Add(listTopBar);
            listTopBar.Add(new Label(i == 0 ? "" : "OR"));



            var listView = new ListView(conditions, 30, MakeItem, (el, ind) => this.BindItem(el, ind, conditions[ind], alternatives.IndexOf(alt)));
            listView.reorderable = true;
            listView.reorderMode = ListViewReorderMode.Animated;
            alternativeElement.Add(listView);

            var buttonsContainer = new VisualElement() { name = "ButtonsContainer"};
            var dropdownMenu = new GenericDropdownMenu();
            foreach (var param in (target as FSMCTransition).StateMachine.Parameters)
            {
                dropdownMenu.AddItem(param.name, false, () => AddCondition(param));
            }
            var dropdownButton = new Button(() => dropdownMenu.DropDown(buttonsContainer.worldBound, buttonsContainer, true));
            dropdownButton.Add(new Image() { image = EditorGUIUtility.FindTexture("Toolbar Plus") });
            dropdownButton.Add(new Image() { image = EditorGUIUtility.FindTexture("Icon Dropdown") });
            var minusButton = new Button(() => RemoveCondition(listView.selectedIndex));
            minusButton.Add(new Image() { image = EditorGUIUtility.FindTexture("Toolbar Minus") });
            buttonsContainer.Add(dropdownButton);
            buttonsContainer.Add(minusButton);
            var optionsButton = new Button(() => {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete"), false, () => RemoveAlternative(outerIndex));
                menu.ShowAsContext();
            }) {name="OptionsButton"};
            buttonsContainer.Add(optionsButton);
            listTopBar.Add(buttonsContainer);

            root.Add(alternativeElement);


            void AddCondition(FSMParameter param)
            {
                if (param.Type == FSMParameterType.Integer)
                {
                    conditions.Add(new FSMCIntegerCondition() { parameter = param as FSMIntegerParameter });
                }
                else if (param.Type == FSMParameterType.Float)
                {
                    conditions.Add(new FSMCFloatCondition() { parameter = param as FSMFloatParameter });
                }
                else if (param.Type == FSMParameterType.Bool)
                {
                    conditions.Add(new FSMCBoolCondition() { parameter = param as FSMBoolParameter });
                }
                else if (param.Type == FSMParameterType.Trigger)
                {
                    conditions.Add(new FSMCBoolCondition() { parameter = param as FSMBoolParameter, comparison = ComparisonType.Equeal, Value = true });
                }
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                listView.Rebuild();
            }
            void RemoveCondition(int index)
            {
                conditions.RemoveAt(index);
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                listView.Rebuild();
            }
        }

        var orButton = new Button(AddAlternative) { text="ADD OR"};
        root.Add(orButton);

        root.AddToClassList("main-container");
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Assets/FSMC/Editor/Editor Resources/FSMCTransitionStyles.uss");
        root.styleSheets.Add(styleSheet);

        return root;
    }

    private void AddAlternative()
    {
        var wrap = new FSMCConditionWrapper();
        wrap.conditions = new();
        alternatives.Add(wrap);
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        root.Clear();
        root.Add(GenerateContent());
    }
    private void RemoveAlternative(int index)
    {
        alternatives.RemoveAt(index);
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        root.Clear();
        root.Add(GenerateContent());
    }

    VisualElement MakeItem()
    {
        var item = new VisualElement();
        item.AddToClassList("list-item");
        return item;
    }

    void BindItem(VisualElement element, int index, FSMCCondition con, int outerIndex)
    {
        element.Clear();
        SerializedProperty prop = serializedObject.FindProperty("conditions").
                GetArrayElementAtIndex(outerIndex).FindPropertyRelative("conditions").GetArrayElementAtIndex(index);
        if (con is FSMCIntegerCondition)
        {
            element.Add(new Label((con as FSMCIntegerCondition).parameter.name));
            var comparison = new EnumField(ComparisonType.Equeal);
            element.Add(comparison);
            comparison.BindProperty(prop.FindPropertyRelative("comparison"));
            var input = new IntegerField();
            element.Add(input);
            input.BindProperty(prop.FindPropertyRelative("Value"));
        }
        else if (con is FSMCFloatCondition)
        {
            element.Add(new Label((con as FSMCFloatCondition).parameter.name));
            var comparison = new EnumField(ComparisonType.Equeal);
            element.Add(comparison);
            comparison.BindProperty(prop.FindPropertyRelative("comparison"));
            var input = new FloatField();
            element.Add(input);
            input.BindProperty(prop.FindPropertyRelative("Value"));
        }
        else if (con is FSMCBoolCondition)
        {
            element.Add(new Label((con as FSMCBoolCondition).parameter.name));
            if ((con as FSMCBoolCondition).parameter.Type != FSMParameterType.Trigger)
            {
                var comparison = new EnumField(ComparisonType.Equeal);
                element.Add(comparison);
                comparison.BindProperty(prop.FindPropertyRelative("comparison"));
                var input = new Toggle();
                element.Add(input);
                input.BindProperty(prop.FindPropertyRelative("Value"));
            }
        }
    }
}