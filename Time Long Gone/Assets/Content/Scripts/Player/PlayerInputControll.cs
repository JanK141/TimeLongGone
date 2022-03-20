using UnityEngine;
using UnityEngine.InputSystem;

namespace Content.Scripts.Player
{
    public class PlayerInputControll : MonoBehaviour
    {
        [SerializeField] private PlayerScript playerScript;
        [SerializeField] private PlayerMovement playerMovement;

        public void WantMove(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("wantMove");

            var x = context.ReadValue<Vector2>().x;
            var z = context.ReadValue<Vector2>().y;

            var xzVector = new Vector3(x, 0, z);
            playerMovement.ProcessMovement(xzVector);
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

        public void WantBlock(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("wantBlock");
            // todo
        }

        public void WantTime(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("wantTime");
            // todo
        }


        public void WantStun(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Debug.Log("wantStun");
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