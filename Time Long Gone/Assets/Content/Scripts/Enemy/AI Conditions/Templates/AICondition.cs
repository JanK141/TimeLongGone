using UnityEngine;

namespace Content.Scripts.Enemy.AI_Conditions.Templates
{
    public abstract class AICondition : ScriptableObject
    {
        public abstract bool Check(GameObject source = null, GameObject target = null);
    }
}
