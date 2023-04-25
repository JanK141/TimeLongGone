using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;
using System.Collections.Generic;
using System;

namespace FSMC.Windows
{
    public class FSMCEditorWindow : EditorWindow
    {
        public FSMController controller;
        public FSMCGraphView graphView;

        private ListView listView;
        
        public void CreateGUI()
        {
            #region SafeCheck
            VisualElement root = rootVisualElement;
            root.Clear();
            if (graphView == null)
            {
                if (controller != null) graphView = new FSMCGraphView(controller);
                else return;
            }else if(controller == null)
            {
                if (graphView.Controller != null) controller = graphView.Controller;
                else return;
            }
            #endregion

            VisualElement splitScreen = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);
            root.Add(splitScreen);

            splitScreen.Add(CreateSideBar());
            
            splitScreen.Add(CreateGraphView());

            AddStyles();

            Undo.undoRedoPerformed += () =>
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                graphView = null;
                CreateGUI();
            };
        }


        private VisualElement CreateSideBar()
        {

            Func<VisualElement> makeItem = () => new FSMCParameter();
            Action<VisualElement, int> bindItem = (e, i) => BindItem(e as FSMCParameter, i);

            listView = new ListView(controller.Parameters, 35, makeItem, bindItem);
            listView.reorderable = true;
            listView.reorderMode = ListViewReorderMode.Animated;
            
            listView.RegisterCallback<KeyDownEvent>(e => { if (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace) DeleteParameter(listView.selectedIndex); });

            VisualElement listContainer = new();
            listContainer.style.width = 150;

            VisualElement topBar = new VisualElement() { name = "TopBar"};
            TextField search = new() {name="SearchField"};
            search.SetPlaceholderText("search...");

            var dropdownMenu = new GenericDropdownMenu();
            dropdownMenu.AddItem("Add Int", false, () => AddParameter("Int", FSMParameterType.Integer, 0));
            dropdownMenu.AddItem("Add Float", false, () => AddParameter("Float", FSMParameterType.Float, 0f));
            dropdownMenu.AddItem("Add Bool", false, () => AddParameter("Bool", FSMParameterType.Bool, false));
            dropdownMenu.AddItem("Add Trigger", false, () => AddParameter("Trigger", FSMParameterType.Trigger, false));

            var dropdownButton = new VisualElement();
            dropdownButton.focusable = true;
            dropdownButton.AddToClassList("CustomButton");
            dropdownButton.RegisterCallback<PointerDownEvent>(e => dropdownMenu.DropDown(topBar.worldBound, topBar, true));
            dropdownButton.Add(new Image() { image = EditorGUIUtility.FindTexture("Toolbar Plus") });
            dropdownButton.Add(new Image() { image = EditorGUIUtility.FindTexture("Icon Dropdown") });

            topBar.Add(search);
            topBar.Add(dropdownButton);

            listContainer.Add(topBar);
            listContainer.Add(listView);

            return listContainer;
        }

        private VisualElement CreateGraphView()
        {
            graphView.StretchToParentSize();
            VisualElement graphContainer = new VisualElement();
            graphContainer.Add(graphView);
            return graphContainer;
        }

        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Assets/FSMC/Editor/Editor Resources/FSMCWindowStyles.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
        }

        #region ListView delegates
        private void BindItem(FSMCParameter elem, int i)
        {
            var item = controller.Parameters[i];
            elem.Q<TextField>(name: "paramName").bindingPath = "m_Name";
            elem.Q<TextField>(name: "paramName").Bind(new SerializedObject(item));
            elem.Q<TextField>(name: "paramName").value = item.name;
            elem.type = item.Type;
            elem.RedrawValue();
            switch (item.Type)
            {
                case FSMParameterType.Integer:
                    elem.Q<IntegerField>(name: "IntegerValue").bindingPath = "_value";
                    elem.Q<IntegerField>(name: "IntegerValue").Bind(new SerializedObject(item as FSMIntegerParameter));
                    elem.Q<IntegerField>(name: "IntegerValue").value = (item as FSMIntegerParameter).Value;
                    break;
                case FSMParameterType.Float:
                    elem.Q<FloatField>(name: "FloatValue").bindingPath = "_value";
                    elem.Q<FloatField>(name: "FloatValue").Bind(new SerializedObject(item as FSMFloatParameter));
                    elem.Q<FloatField>(name: "FloatValue").value = (item as FSMFloatParameter).Value;
                    break;
                case FSMParameterType.Bool:
                    elem.Q<Toggle>(name: "BoolValue").bindingPath = "_value";
                    elem.Q<Toggle>(name: "BoolValue").Bind(new SerializedObject(item as FSMBoolParameter));
                    elem.Q<Toggle>(name: "BoolValue").value = (item as FSMBoolParameter).Value;
                    break;
                case FSMParameterType.Trigger:
                    elem.Q<Toggle>(name: "TriggerValue").bindingPath = "_value";
                    elem.Q<Toggle>(name: "TriggerValue").Bind(new SerializedObject(item as FSMBoolParameter));
                    elem.Q<Toggle>(name: "TriggerValue").value = (item as FSMBoolParameter).Value;
                    break;
            };
            elem.parent.AddManipulator(new ContextualMenuManipulator(
                e => e.menu.AppendAction("Delete", menuEvent => DeleteParameter(i)))
            );
        }
        private void DeleteParameter(int index)
        {
            Undo.SetCurrentGroupName("Delete parameter");
            int group = Undo.GetCurrentGroup();

            var param = controller.Parameters[index];
            Undo.RegisterCompleteObjectUndo(controller, "Delete parameter");
            Undo.DestroyObjectImmediate(param);
            controller.Parameters.Remove(param);
            listView.RefreshItems();

            Undo.CollapseUndoOperations(group);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private void AddParameter(string name, FSMParameterType type, object value)
        {
            Undo.SetCurrentGroupName("Add parameter");
            int group = Undo.GetCurrentGroup();

            Undo.RegisterCompleteObjectUndo(controller, "Add parameter");
            controller.Parameters.Add(FSMParameter.CreateParameter(name, type, value, controller));
            listView.RefreshItems();

            Undo.CollapseUndoOperations(group);
        }
        #endregion

        #region Opening
        [OnOpenAsset(0)]
        public static bool OnOpen(int instanceID, int line)
        {
            FSMController controller = EditorUtility.InstanceIDToObject(instanceID) as FSMController;
            if (controller == null) return false;

            /*FSMCGraphView graphView = FSMCcashedGraphs.GetGraph(instanceID);
            if (graphView == null) return false;*/
            OpenWindow(controller);
            return true;
        }
        public static FSMCEditorWindow OpenWindow(FSMController contr)
        {
            FSMCEditorWindow wnd = GetWindow<FSMCEditorWindow>();
            wnd.minSize = new Vector2(600, 400);
            wnd.titleContent = new GUIContent(contr.name);
            //wnd.graphView = graphView;
            wnd.controller = contr;
            wnd.CreateGUI();
            return wnd;
        }
            #endregion
    }

}
