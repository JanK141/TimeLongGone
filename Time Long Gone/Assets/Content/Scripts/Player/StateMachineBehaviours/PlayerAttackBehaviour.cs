using UnityEngine;

namespace Content.Scripts.Player.StateMachineBehaviours
{
    public class PlayerAttackBehaviour : StateMachineBehaviour
    {
        [SerializeField] [Tooltip("Put 0 to not affect move speed")] private float moveSpeed = 0;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var player = PlayerScript.Instance;
            player.movementScript.rotateSlow = true;
            if(moveSpeed!=0)player.movementScript.Speed = moveSpeed;
        }
    }
}
