using Enemy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy1))]
public class Enemy1Editor : UnityEditor.Editor
{
    private int stage = 0;

    private void OnSceneGUI()
    {
        Handles.BeginGUI();
        if (Application.isPlaying)
        {
            GUILayout.BeginArea(new Rect(20, 20, 300, 250));
            var serprop = serializedObject.FindProperty("currSM").objectReferenceValue as StateMachine;
            GUI.Box(EditorGUILayout.BeginVertical(), GUIContent.none);

            if (serprop == serializedObject.FindProperty("Stages").GetArrayElementAtIndex(0).objectReferenceValue as StateMachine)
                DrawInPlay1(serprop);
            /*else if (serprop == serializedObject.FindProperty("Stages").GetArrayElementAtIndex(1).objectReferenceValue as StateMachine)
                DrawInPlay2(serprop);
            else if (serprop == serializedObject.FindProperty("Stages").GetArrayElementAtIndex(2).objectReferenceValue as StateMachine)
                DrawInPlay3(serprop);*/

            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        }
        else
        {
            GUILayout.BeginArea(new Rect(20, 20, 300, 250));
            GUI.Box(EditorGUILayout.BeginVertical(), GUIContent.none);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous stage")) stage = Mathf.Clamp(stage - 1, 0, serializedObject.FindProperty("Stages").arraySize - 1);
            if (GUILayout.Button("Next stage")) stage = Mathf.Clamp(stage + 1, 0, serializedObject.FindProperty("Stages").arraySize - 1);
            EditorGUILayout.EndHorizontal();

            if (stage == 0)
                DrawInEdit1(serializedObject.FindProperty("Stages").GetArrayElementAtIndex(stage).objectReferenceValue as StateMachine);
            /*else if (stage == 1)
                DrawInEdit2(serializedObject.FindProperty("Stages").GetArrayElementAtIndex(stage).objectReferenceValue as StateMachine);
            else if (stage == 2)
                DrawInEdit3(serializedObject.FindProperty("Stages").GetArrayElementAtIndex(stage).objectReferenceValue as StateMachine);*/

            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        }
        Handles.EndGUI();
    }

    private void DrawSMProp(string label, string text)
    {
        GUI.color = Color.white;
        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        GUILayout.FlexibleSpace();
        GUILayout.Label(text);
        GUILayout.EndHorizontal();
    }

    #region InPlay
    private void DrawInPlay1(StateMachine serprop)
    {
        DrawSMProp("Stage", "1");
        DrawSMProp("CurrentState", serprop.GetCurrentState().stateName);
        DrawSMProp("PlayerAvgDeltaPos", serprop.GetFloat("PlayerAvgDeltaPos").ToString("F0"));
        DrawSMProp("AngleToPlayer", serprop.GetFloat("AngleToPlayer").ToString("F0"));
        DrawSMProp("DistanceToPlayer", serprop.GetFloat("Distance").ToString("F0"));
        DrawSMProp("TimeSinceRest", serprop.GetFloat("TimeSinceRest").ToString("F0"));
        DrawSMProp("TimeSinceCharge", serprop.GetFloat("TimeSinceCharge").ToString("F0"));
        DrawSMProp("AttacksInCombo", serprop.GetInt("AttacksInCombo").ToString());
        DrawSMProp("HealthDelta", serprop.GetFloat("HealthDelta").ToString("F0"));
        DrawSMProp("BreakTime", serprop.GetFloat("BreakTime").ToString("F0"));
        DrawSMProp("DistanceToProjectileSpot", serprop.GetFloat("DistanceToProjectileSpot").ToString("F0"));
    }
    #endregion

    #region InEdit
    private void DrawInEdit1(StateMachine serprop)
    {
        serprop.SetFloat("AngleToPlayer", Vector3.SignedAngle(FindObjectOfType<Player.Player>().transform.position - (target as Enemy1).transform.position, (target as Enemy1).transform.forward, Vector3.up));
        serprop.SetFloat("Distance", Vector3.Distance((target as Enemy1).transform.position, FindObjectOfType<Player.Player>().transform.position));
        var projectilesSpots = GameObject.FindGameObjectsWithTag("Projectiles Spot").Select(o => o.transform.position).ToList();
        serprop.SetFloat("DistanceToProjectileSpot", projectilesSpots.Select(p => Vector3.Distance((target as Enemy1).transform.position, p)).Min());

        DrawSMProp("Stage", "1");
        DrawSMProp("AngleToPlayer", serprop.GetFloat("AngleToPlayer").ToString("F0"));
        DrawSMProp("DistanceToPlayer", serprop.GetFloat("Distance").ToString("F0"));
        DrawSMProp("DistanceToProjectileSpot", serprop.GetFloat("DistanceToProjectileSpot").ToString("F0"));
    }
    #endregion
}
