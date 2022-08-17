using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Player.StateMachineBehaviours
{
    public class PlayerResetBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var player = PlayerScript.Instance;
            player.movementScript.rotateSlow = false;
            player.transform.DOKill();
            player.movementScript.ResetSpeed();
            player.movementScript.canMove = true;
            player.movementScript.canRotate = true;
            player.combat.CanAttack = true;
            player.playerInput.CanStun = true;
        }
    }
}
