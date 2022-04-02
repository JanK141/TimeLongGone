using Content.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Content.Scripts.Inputs
{
    public class PlayerInput : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] [Range(-1, 0)] private float holdTreshhold = -0.25f;
        #endregion

        #region Private Variables
        private bool _isCharging = false;
        private float _holdTime;
        private bool _isBlocking = false;
        #endregion

        private PlayerScript player;

        void Start()
        {
            player = PlayerScript.Instance;
            _holdTime = holdTreshhold;
        }

        void Update()
        {
            if (_isCharging)
            {
                _holdTime += Time.deltaTime;
                if(_holdTime>=0 && !player.combat.IsCharging) player.combat.StartCharging();
            }
            if(_isBlocking && !player.combat.IsBlocking) player.combat.Block(true);
        }

        public void WantMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var x = context.ReadValue<Vector2>().x;
                var y = context.ReadValue<Vector2>().y;

                player.movementScript.InputVector = new Vector2(x, y);
            }
            else if (context.canceled)
                player.movementScript.InputVector = new Vector2(0, 0);
        }

        public void WantJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                player.movementScript.ProcessJump();
        }

        public void WantChargeAttack(InputAction.CallbackContext context)
        {
            if (context.started && !_isBlocking)
            {
                _isCharging = true;
            }
            else if (context.performed && _isCharging)
            {
                if(_holdTime>0)player.combat.ChargedAttack(_holdTime);
                else player.combat.Attack();
                ResetHold();
            }
            else if (context.canceled && _isCharging)
            {
                if (_holdTime<0) player.combat.Attack();
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
            if (context.started)
            {
                _isBlocking = true;
                player.combat.Block(true);
                if(_isCharging) ResetHold();
            }
            else if((context.performed || context.canceled) && _isBlocking)
            {
                _isBlocking = false;
                player.combat.Block(false);
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
                player.movementScript.ProcessDash();
        }

        public void ResetHold()
        {
            _isCharging = false;
            _holdTime = holdTreshhold;
        }

    }
}