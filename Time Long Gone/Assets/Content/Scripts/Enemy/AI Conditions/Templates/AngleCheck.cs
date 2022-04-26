using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Conditions/Angle Check")]
public class AngleCheck : AICondition
{
    [SerializeField] [Tooltip("Absolute value")] private float angle;
    [SerializeField] [Tooltip("As default, checks if angle is lower than given. Set True to inverse this behavior")] private bool higherThan = false;
    public override bool Check(GameObject source, GameObject target)
    {
        if (source == null || target == null) return false;
        Vector3 targetDir = target.transform.position - source.transform.position;
        Vector3 forward = source.transform.forward;
        float res = Vector3.SignedAngle(targetDir, forward, Vector3.up);
        bool bres = (res >= -angle && res <= angle);
        return (higherThan) ? !bres : bres;
    }
}
