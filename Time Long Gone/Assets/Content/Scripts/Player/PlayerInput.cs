using UnityEngine;
using UnityEngine.InputSystem;

namespace Content.Scripts.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private PlayerMain playerScript;

        private static float Vertical
        {
            get
            {
                var keyboard = Keyboard.current;
                var vertical = 0;

                if (keyboard.wKey.isPressed)
                    vertical = 1;
                else if (keyboard.sKey.isPressed) vertical = -1;

                return vertical;
            }
        }

        private static float Horizontal
        {
            get
            {
                var keyboard = Keyboard.current;
                var horizontal = 0;

                if (keyboard.dKey.isPressed)
                    horizontal = 1;
                else if (keyboard.aKey.isPressed) horizontal = -1;

                return horizontal;
            }
        }


        //   [SerializeField] private float gravity = -30;
        //   [SerializeField] private float jumpHeight = 10;


        private static PlayerInput Instance;
        private Rigidbody _rb;
        private CharacterController _controller;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            Instance = this;
            _controller = GetComponent<CharacterController>();
        }

        public void Move(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            var direction = new Vector3(Horizontal, 0f, Vertical).normalized;

            if (!(direction.magnitude >= 0.1f)) return;

            var targetAngel = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngel, 0f);
            _controller.Move(direction * playerScript.walkSpeed * Time.deltaTime);
        }

        public void Dash(InputAction.CallbackContext context)
        {
        }

        public void Jump(InputAction.CallbackContext context)
        {
            if (!context.performed || playerScript.inTheAir) return;

            Debug.Log("Jump");
            _rb.AddForce(Vector3.up * 3f, ForceMode.Impulse);
        }


        public void attack(InputAction.CallbackContext context)
            =>
                playerScript.isAttacking = true;

        public void ChargeAttack(InputAction.CallbackContext context)
            =>
                playerScript.isChargingAttack = true;

        public void Block(InputAction.CallbackContext context)
            => playerScript.isBlocking = true;


        public void Stun(InputAction.CallbackContext context)
        {
        }
    }
}