using UnityEngine;

namespace Content.Scripts.Player.StateMachineBehaviours
{
    public class PreAttackBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
            => PlayerScript.Instance.combat.InvokeAttackReset(stateInfo.length);
    }
}
