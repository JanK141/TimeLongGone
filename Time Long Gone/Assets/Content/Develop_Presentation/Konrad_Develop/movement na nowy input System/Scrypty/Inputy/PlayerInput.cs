using Content.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Content.Scripts.Inputs
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private PlayerScript playerScript;
        [SerializeField] private PlayerMovement playerMovement;

        public void WantMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log("wantMove");
                var x = context.ReadValue<Vector2>().x;
                var z = context.ReadValue<Vector2>().y;

                playerMovement.InputVector = new Vector3(x, 0, z);
            }
            else if (context.canceled)
                playerMovement.InputVector = new Vector3(0, 0, 0);
        }

        public void WantJump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("wantJump");
            playerScript.movementScript.ProcessJump();
        }

        public void WantAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("wantAttack");
            // todo
        }

        public void WantChargeAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("wantChargeAttack");
            // todo
        }
        
        public void WantStunAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("wantStun");
            // todo
        }

        public void WantBlock(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("wantBlock");
            // todo
        }

        public void WantTimeManipulating(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("wantTime");
            // todo
        }

        public void WantDash(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("wantDash");
            playerScript.movementScript.ProcessDash();
        }
    }
}