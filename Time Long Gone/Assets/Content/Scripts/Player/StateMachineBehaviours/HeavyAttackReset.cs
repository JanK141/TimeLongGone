using UnityEngine;

namespace Content.Scripts.Player.StateMachineBehaviours
{
    public class HeavyAttackReset : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
            => PlayerScript.Instance.combat.StickToGround(true);
    }
}
