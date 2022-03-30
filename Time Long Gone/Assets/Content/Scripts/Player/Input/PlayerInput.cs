using Content.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Content.Scripts.Inputs
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] [Range(-1, 0)] private float holdTreshhold = -0.25f;
        [SerializeField] private float blockCD = 0.5f;

        private PlayerScript playerScript;

        private bool isCharging = false;
        private float holdTime;
        private bool isBlocking = false;
        private bool canBlock = true;

        void Start()
        {
            playerScript = PlayerScript.Instance;
            holdTime = holdTreshhold;
        }

        void Update()
        {
            if (isCharging)
            {
                holdTime += Time.deltaTime;
                if(holdTime>=0)playerScript.anim.SetBool("isCharging", true);
            }
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
            if (context.started && !isBlocking)
            {
                isCharging = true;
            }
            else if (context.performed && isCharging)
            {
                if(holdTime>0)playerScript.combat.ChargedAttack(holdTime);
                else playerScript.combat.Attack();
                ResetHold();
            }
            else if (context.canceled && isCharging)
            {
                if (holdTime<0) playerScript.combat.Attack();
                ResetHold();
            }
        }
        
        public void WantStunAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            // todo
        }

        public void WantBlock(InputAction.CallbackContext context)
        {
            if(!canBlock) return;
            if (context.started)
            {
                isBlocking = true;
                playerScript.anim.SetBool("Blocking", true);
                if(isCharging) ResetHold();
            }
            else if((context.performed || context.canceled) && isBlocking)
            {
                isBlocking = false;
                playerScript.anim.SetBool("Blocking", false);
                Invoke(nameof(ResetBlock), blockCD);
            }
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
            playerScript.anim.SetBool("isCharging", false);
        }

        void ResetBlock() => canBlock = true;
    }
}