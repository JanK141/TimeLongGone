using System.Collections;
using UnityEngine;

namespace Content.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private static PlayerMovement _instance;

        [Header("Basic movement")] [SerializeField]
        private float speed = 10;

        [SerializeField] private float gravity = -30;
        [SerializeField] private float jumpHeight = 1;
        [Header("Dashing")] [SerializeField] private float dashCd = 1f;

        [SerializeField] [Tooltip("How long dash animation is")]
        private float dashTime = 0.25f;

        [SerializeField] [Tooltip("What product of speed is applied every frame of a dash")]
        private float dashMoveMultiplier = 2f;

        [Header("Dash i-frames")]
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("In what percent of dash animation i-frames start")]
        private float iframesStart;

        [SerializeField] [Range(0, 1)] [Tooltip("In what percent of dash animation i-frames end")]
        private float iframesEnd = 0.3f;

        [Space] [SerializeField] private LayerMask groundMask;

        public float FramesStart => iframesStart;
        public float FramesEnd => iframesEnd;

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public float DashTime => dashTime;

        public bool CanDash
        {
            get => _canDash;
            set => _canDash = value;
        }

        public bool CanMove
        {
            get => _canMove;
            set => _canMove = value;
        }

        private bool IsInvincible { get; set; }

        public Vector3 InputVector { get; set; }

        public Vector3 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        public float Gravity => gravity;

        private Vector3 _velocity;
        private Vector3 _move;
        private bool _isGrounded;
        private bool _canDash = true;
        private bool _canMove = true;

        private CharacterController _controller;

        private void Awake()
        {
            _instance = this;
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            ProcessGravity();
            ProcessMovement();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void ProcessMovement()
        {
            var vertical = InputVector.x;
            var horizontal = InputVector.z;

            _move = new Vector3(vertical * Mathf.Sqrt(1 - horizontal * horizontal * 0.5f), 0,
                horizontal * Mathf.Sqrt(1 - vertical * vertical * 0.5f));

            if (!_canMove || !(_move.magnitude > 0.05)) return;

            _controller.Move(_move * (speed * Time.deltaTime));
            transform.LookAt(transform.position + _move);
        }


        public void ProcessDash()
        {
            if (!_canDash) return;
            _canDash = false;
            StartCoroutine(nameof(Dash));
            Invoke(nameof(ResetDashCd), dashCd);
        }

        public void ProcessJump()
        {
            if (!_isGrounded) return;
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        private void ProcessGravity()
        {
            var position = transform.position;
            _isGrounded = Physics.CheckSphere(new Vector3(position.x,
                    position.y - _controller.height / 2,
                    position.z),
                0.4f,
                groundMask);

            if (_isGrounded && _velocity.y < 0)
                _velocity = new Vector3(0, -2f, 0);

            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        private IEnumerator Dash()
        {
            _canMove = false;
            var motion = _move.normalized;
            var time = 0f;
            while (time < dashTime)
            {
                if (!IsInvincible && time >= iframesStart * dashTime) IsInvincible = true;
                if (IsInvincible && time >= iframesEnd * dashTime) IsInvincible = false;
                time += Time.deltaTime;
                _controller.Move(motion * speed * dashMoveMultiplier * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            _canMove = true;
        }

        private void ResetDashCd() => _canDash = true;
    }
}