using UnityEngine;
using UnityEngine.InputSystem;

namespace Content.Scripts.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private PlayerScript playerScript;

        public void WantMove(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            var keyboard = Keyboard.current;

            if (keyboard.wKey.isPressed)
            {
                playerScript.isUpPressed = true;
                Debug.Log("wantMoveUp");
            }

            if (keyboard.sKey.isPressed)
            {
                playerScript.isDownPressed = true;
                Debug.Log("wantMoveDown");
            }

            if (keyboard.aKey.isPressed)
            {
                playerScript.isLeftPressed = true;
                Debug.Log("wantMoveLeft");
            }

            if (keyboard.dKey.isPressed)
            {
                playerScript.isRightPressed = true;
                Debug.Log("wantMoveRight");
            }
        }

        public void WantDash(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            playerScript.isDashPressed = true;
            Debug.Log("wantDash");
        }

        public void WantJump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            playerScript.isJumpPressed = true;
            Debug.Log("wantJump");
        }

        public void WantAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            playerScript.isAttackPressed = true;
            Debug.Log("wantAttack");
        }

        public void WantChargeAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            playerScript.isChargeAttackPressed = true;
            Debug.Log("wantChargeAttack");
        }

        public void WantBlock(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            playerScript.isBlockPressed = true;
            Debug.Log("wantBlock");
        }

        public void WantTime(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            playerScript.isTimePressed = true;
            Debug.Log("wantTime");
        }


        public void WantStun(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            playerScript.CurrentCombat = PlayerScript.CombatStatus.Stun;
            Debug.Log("wantStun");
        }
    }
}