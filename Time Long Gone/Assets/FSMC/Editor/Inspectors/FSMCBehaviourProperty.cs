using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(FSMCBehaviour))]
public class FSMCBehaviourProperty : PropertyDrawer
{
    /*public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        var sobj = new SerializedObject(property.objectReferenceValue);
        var e = sobj.GetIterator();
        *//*EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                e);*//*
        int i = 0;
        while (e.NextVisible(true))
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y + i * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),
                e);
            i++;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var e = property.GetEnumerator();
        int i = 0;
        while (e.MoveNext())
        {
            *//*EditorGUI.PropertyField(new Rect(position.x, position.y + i * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),
            e.Current as SerializedProperty);*//*
            i++;
        }
        //return i * EditorGUIUtility.singleLineHeight;
        return 300f;
    }*/

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();

        var sobj = new SerializedObject(property.objectReferenceValue);
        var e = sobj.GetIterator();

        while (e.NextVisible(true))
        {
            root.Add(new PropertyField(e));
        }
        return root;
    }

}
