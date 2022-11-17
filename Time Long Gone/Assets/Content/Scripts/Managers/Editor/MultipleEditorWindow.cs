using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;

public class MultipleEditorWindow : EditorWindow
{
    protected SerializedObject[] serializedObjects;
    protected SerializedObject currentSerializedObject;

    protected void DrawSerializedObject(SerializedObject serialized, bool drawChildren)
    {
        SerializedProperty prop = serialized.GetIterator();
        if (prop.NextVisible(drawChildren))
        {
            do
            {
                EditorGUILayout.PropertyField(serialized.FindProperty(prop.name), drawChildren);
            }while (prop.NextVisible(false));
        }
       
    }

    protected void DrawSidebar()
    {
        for(int i = 0; i < serializedObjects.Length; i++)
        {
            if (GUILayout.Button(serializedObjects[i].targetObject.name))
            {
                currentSerializedObject = serializedObjects[i];
            }
        }
    }
}
