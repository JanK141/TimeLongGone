using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Enemy.AI_Conditions.Templates;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Conditions/Angle Check")]
public class AngleCheck : AICondition
{
    [SerializeField] [Tooltip("Absolute value")] private float angle;
    [SerializeField] [Tooltip("As default, checks if angle is lower than given. Set True to inverse this behavior")] private bool higherThan;
    public override bool Check(GameObject source, GameObject target)
    {
        if (source == null || target == null) return false;
        var targetDir = target.transform.position - source.transform.position;
        var forward = source.transform.forward;
        var res = Vector3.SignedAngle(targetDir, forward, Vector3.up);
        var bres = res >= -angle && res <= angle;
        return higherThan ? !bres : bres;
    }
}
