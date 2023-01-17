using Content.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ManagersWindow : MultipleEditorWindow
{
    [MenuItem("Window/Managers")]
    public static void Open()
    {
        GetWindow<ManagersWindow>("Managers");        
    }

    private void OnEnable()
    {
        serializedObjects = Resources.LoadAll("Managers").Select(o => new SerializedObject((o as GameObject).GetComponents(typeof(MonoBehaviour))[0])).ToArray();
    }

    Vector2 scroll;

    private void OnGUI()
    {
        using(new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUILayout.VerticalScope("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true)))
            {
                DrawSidebar();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Refresh"))
                {
                    this.OnEnable();
                    currentSerializedObject = null;
                    Repaint();
                }
            }
            using (new EditorGUILayout.VerticalScope("box", GUILayout.ExpandHeight(true)))
            {
                if(currentSerializedObject != null)
                {
                    EditorGUILayout.LabelField(currentSerializedObject.targetObject.name, EditorStyles.boldLabel);
                    scroll = EditorGUILayout.BeginScrollView(scroll, GUIStyle.none);
                    DrawSerializedObject(currentSerializedObject, true);
                    EditorGUILayout.EndScrollView();
                    currentSerializedObject.ApplyModifiedProperties();
                }
                else
                {
                    EditorGUILayout.LabelField("Select manager to inspect");
                }
            }
        }
    }
}
