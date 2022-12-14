using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Level1DataManager.Sound))]
public class SoundPropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 4 * EditorGUIUtility.singleLineHeight-10;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var nameRectLabel = new Rect(position.x, position.y, 50, EditorGUIUtility.singleLineHeight);
        var nameRect = new Rect(position.x+50, position.y, (2*(position.width/3))-55, EditorGUIUtility.singleLineHeight);
        var clipRect = new Rect(position.x + 2*position.width/3, position.y, position.width / 3, EditorGUIUtility.singleLineHeight);
        var volumeRectLabel = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, 50, EditorGUIUtility.singleLineHeight);
        var volumeRect = new Rect(position.x+50, position.y + EditorGUIUtility.singleLineHeight, position.width-50, EditorGUIUtility.singleLineHeight);
        var pitchRectLabel = new Rect(position.x, position.y + 2*EditorGUIUtility.singleLineHeight, 50, EditorGUIUtility.singleLineHeight);
        var pitchRectValue = new Rect(position.x+50, position.y + 2*EditorGUIUtility.singleLineHeight, 70, EditorGUIUtility.singleLineHeight);
        var pitchRect = new Rect(position.x+125, position.y + 2*EditorGUIUtility.singleLineHeight, position.width-125, EditorGUIUtility.singleLineHeight);

        EditorGUI.LabelField(nameRectLabel, "Name");
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);
        EditorGUI.PropertyField(clipRect, property.FindPropertyRelative("clip"), GUIContent.none);
        EditorGUI.LabelField(volumeRectLabel, "Volume");
        EditorGUI.PropertyField(volumeRect, property.FindPropertyRelative("volume"), GUIContent.none);
        float min= property.FindPropertyRelative("minPitch").floatValue;
        float max= property.FindPropertyRelative("maxPitch").floatValue;
        EditorGUI.LabelField(pitchRectLabel, new GUIContent("Pitch", "Select range that will be randomly selected"));
        using (new EditorGUI.DisabledGroupScope(true)) EditorGUI.TextField(pitchRectValue, "[" + min.ToString("F1") + "-" + max.ToString("F1") + "]");
        EditorGUI.MinMaxSlider(pitchRect, GUIContent.none, ref min, ref max, 0, 1);
        property.FindPropertyRelative("minPitch").floatValue = min;
        property.FindPropertyRelative("maxPitch").floatValue = max;

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
