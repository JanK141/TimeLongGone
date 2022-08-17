using UnityEngine;

namespace Content.Scripts.Player.assembly_Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerVariables variables;
        [SerializeField] private LayerMask ground;


        void InstaRotate()
        {
        }

        void SlowRotate()
        {
        }

        void MoveNormal()
        {
        }

        void MoveSlow()
        {
        }

        void MoveGravityOnly()
        {
        }

        void ResetBlock()
        {
        }

        void ResetDash()
        {
        }

        void ResetAttack()
        {
        }

        void UpdateAnimator()
        {
            float horizontalVelocity, verticalVelocity;
            bool isGrounded, isBlocking;
        }
    }
}