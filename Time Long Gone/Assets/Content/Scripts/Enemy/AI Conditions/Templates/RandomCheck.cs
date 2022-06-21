using Content.Scripts.Enemy.AI_Conditions.Templates;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Conditions/Random Check")]
public class RandomCheck : AICondition
{
    [SerializeField] [Range(0, 1)] private float chance;

    public override bool Check(GameObject source, GameObject target) => Random.value <= chance;
}
