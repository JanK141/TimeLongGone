using Content.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Content.Scripts.Inputs
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] [Range(-1, 0)] private float holdTreshhold = -0.25f;

        private PlayerScript playerScript;

        private bool isCharging = false;
        private bool isOnPressCd = false;
        private float holdTime;
        private float initialMoveSpeed;

        void Start()
        {
            playerScript = PlayerScript.Instance;
            holdTime = holdTreshhold;
            initialMoveSpeed = playerScript.movementScript.Speed;
        }

        void Update()
        {
            if (isCharging) holdTime += Time.deltaTime;
        }

        public void WantMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var x = context.ReadValue<Vector2>().x;
                var z = context.ReadValue<Vector2>().y;

                playerScript.movementScript.InputVector = new Vector3(x, 0, z);
            }
            else if (context.canceled)
                playerScript.movementScript.InputVector = new Vector3(0, 0, 0);
        }

        public void WantJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                playerScript.movementScript.ProcessJump();
        }

        public void WantChargeAttack(InputAction.CallbackContext context)
        {
            if(isOnPressCd) return;
            if (context.started)
            {
                isCharging = true;
                playerScript.movementScript.Speed *= 0.4f;
            }
            else if (context.performed)
            {
                playerScript.movementScript.Speed = initialMoveSpeed;
                if(holdTime>0)playerScript.combat.ChargedAttack(holdTime);
                else playerScript.combat.Attack();
                isOnPressCd = true;
                Invoke(nameof(ResetHold), -holdTreshhold);
            }
            else if (context.canceled)
            {
                playerScript.movementScript.Speed = initialMoveSpeed;
                if (holdTime<0) playerScript.combat.Attack();
                isOnPressCd = true;
                Invoke(nameof(ResetHold), -holdTreshhold);
            }
        }
        
        public void WantStunAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            // todo
        }

        public void WantBlock(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            // todo
        }

        public void WantTimeManipulating(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            // todo
        }

        public void WantDash(InputAction.CallbackContext context)
        {
            if (!context.performed)
                playerScript.movementScript.ProcessDash();
        }

        void ResetHold()
        {
            isCharging = false;
            holdTime = holdTreshhold;
            isOnPressCd = false;
        }

    }
}