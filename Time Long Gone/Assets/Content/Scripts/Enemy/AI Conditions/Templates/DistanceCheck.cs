using UnityEngine;

namespace Content.Scripts.Enemy.AI_Conditions.Templates
{
    [CreateAssetMenu(menuName = "AI Conditions/Distance Check")]
    public class DistanceCheck : AICondition
    {
        [SerializeField] private float distance;
        [SerializeField] [Tooltip("As default, checks if distance is lower than given. Set True to inverse this behavior")] private bool higherThan;
        public override bool Check(GameObject source, GameObject target)
        {
            if (source == null || target == null) return false;
            var res = Vector3.Distance(source.transform.position, target.transform.position) < distance;
            return higherThan ? !res : res;
        }
    }
}
