using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class TransformRandomizer : EditorWindow
{
    [MenuItem("Tools/Transform Randomizer")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TransformRandomizer));
    }

    private void OnEnable()
    {
        autoRepaintOnSceneChange = true;
        showScales = new AnimBool(false);
        showScales.valueChanged.AddListener(base.Repaint);
    }
    private void OnDisable()
    {
        autoRepaintOnSceneChange = false;
    }


    private bool rotateX = true;
    private bool rotateY = true;
    private bool rotateZ = true;
    private bool scaleX = true;
    private bool scaleY = true;
    private bool scaleZ = true;
    private bool scaleUniform = true;
    private float minRot = -360;
    private float maxRot = 360;
    private float minScale = 0;
    private float maxScale = 1;

    AnimBool showScales;

    private void OnGUI()
    {
        Transform[] selected = Selection.transforms;
        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label(selected.Length + " selected objects");
            GUILayout.FlexibleSpace();
        }
        GUILayout.FlexibleSpace();
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label(new GUIContent("Rotation constraints", "Toggle which axis to include"));
            GUILayout.FlexibleSpace();
            rotateX = GUILayout.Toggle(rotateX, "X");
            rotateY = GUILayout.Toggle(rotateY, "Y");
            rotateZ = GUILayout.Toggle(rotateZ, "Z");
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing * 2);

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label(new GUIContent("Between [ " +minRot.ToString("F1") +" ; "+maxRot.ToString("F1")+" ]"));
            GUILayout.FlexibleSpace();
            EditorGUILayout.MinMaxSlider(ref minRot, ref maxRot, -360f, 360f);
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing * 2);
        using (new EditorGUI.DisabledGroupScope(selected.Length == 0))
        {
            if(GUILayout.Button("Randomize Rotation"))
            {
                Undo.RecordObjects(selected, "Randomized rotation");
                RandomRotation(selected);
            }
        }
        GUILayout.FlexibleSpace();
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label(new GUIContent("Uniform Scale", "Should scale be the same in all axis"));
            scaleUniform = GUILayout.Toggle(scaleUniform, GUIContent.none);
            showScales.target = !scaleUniform;
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing * 2);
        using (var group = new EditorGUILayout.FadeGroupScope(showScales.faded))
        {
            if (group.visible)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(new GUIContent("Scale constraints", "Toggle which axis to include"));
                    GUILayout.FlexibleSpace();
                    scaleX = GUILayout.Toggle(scaleX, "X");
                    scaleY = GUILayout.Toggle(scaleY, "Y");
                    scaleZ = GUILayout.Toggle(scaleZ, "Z");
                    GUILayout.FlexibleSpace();
                }
            }
        }
        EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing * 2);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label(new GUIContent("Between"));
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent("Min"));
            minScale = EditorGUILayout.FloatField(minScale);
            GUILayout.Label(new GUIContent("Max"));
            maxScale = EditorGUILayout.FloatField(maxScale);
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing * 2);
        using (new EditorGUI.DisabledGroupScope(selected.Length == 0))
        {
            if(GUILayout.Button("Randomize Scale"))
            {
                Undo.RecordObjects(selected, "Randomized scale");
                RandomScale(selected);
            }
        }
        GUILayout.FlexibleSpace();
    }

    private void RandomRotation(Transform[] tranforms)
    {
        foreach(Transform t in tranforms)
        {
            var rand = new Vector3(
                (rotateX)?Random.Range(minRot, maxRot):t.rotation.eulerAngles.x, 
                (rotateY)?Random.Range(minRot, maxRot):t.rotation.eulerAngles.y, 
                (rotateZ)?Random.Range(minRot, maxRot):t.rotation.eulerAngles.z
                );
            t.rotation = Quaternion.Euler(rand);
        }
    }
    private void RandomScale(Transform[] tranforms)
    {
        foreach(var t in tranforms)
        {
            if (scaleUniform)
            {
                var rand = Random.Range(minScale, maxScale);
                t.localScale = new Vector3(rand, rand, rand);
            }
            else
            {
                var rand = new Vector3(
                    (scaleX)?Random.Range(minScale, maxScale):t.localScale.x,
                    (scaleY)?Random.Range(minScale, maxScale):t.localScale.y,
                    (scaleZ)?Random.Range(minScale, maxScale):t.localScale.z
                    );
                t.localScale = rand;
            }
        }
    }
}
