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
                playerScript.isUpPressed = true;
            if (keyboard.sKey.isPressed)
                playerScript.isDownPressed = true;
            if (keyboard.aKey.isPressed)
                playerScript.isLeftPressed = true;
            if (keyboard.dKey.isPressed)
                playerScript.isRightPressed = true;
        }

        public void WantDash(InputAction.CallbackContext context)
        {
            if (context.performed)
                playerScript.isDashPressed = true;
        }

        public void WantJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                playerScript.isJumpPressed = true;
        }

        public void WantAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                playerScript.isAttackPressed = true;
        }

        public void WantChargeAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                playerScript.isChargeAttackPressed = true;
        }

        public void WantBlock(InputAction.CallbackContext context)
        {
            if (context.performed)
                playerScript.isBlockPressed = true;
        }

        public void WantTime(InputAction.CallbackContext context)
        {
            if (context.performed)
                playerScript.isTimePressed = true;
        }


        public void WantStun(InputAction.CallbackContext context)
            =>
                playerScript.CurrentCombat = PlayerScript.CombatStatus.Stun;
    }
}