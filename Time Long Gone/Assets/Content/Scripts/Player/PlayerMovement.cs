using UnityEngine;
using UnityEngine.InputSystem;

namespace Content.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private PlayerScript playerScript;

        public CharacterController controller;

        public float speed = 12f;
        public float gravity = -9.81f;
        public float jumpHeight = 3f;


        // propraiteries

        private float Vertical
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

        private float Horizontal
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


        public Transform groundCheck;
        public float groundDistance = 0.4f;
        public LayerMask groundMask;
        private Vector3 velocity;
        private bool isGrounded;

        private void Update()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            var x = Vertical;
            var z = Horizontal;

            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;

            var transform1 = transform;
            var move = transform1.right * x + transform1.forward * z;

            controller.Move(move * (speed * Time.deltaTime));
            velocity.y += gravity * speed * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            if (playerScript.isJumpPressed && isGrounded)
                velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }
    }
}