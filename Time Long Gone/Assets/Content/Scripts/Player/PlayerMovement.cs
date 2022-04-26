using System.Collections;
using Content.Scripts.Camera;
using Content.Scripts.Enemy;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace Content.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        #region Inspector Fields

        [Header("Basic movement")] [SerializeField]
        private float speed = 10;

        [SerializeField] private float gravity = -30;
        [SerializeField] private float jumpHeight = 1;

        [Header("Dashing")] [SerializeField] private float dashCd = 1f;

        [SerializeField] [Tooltip("How long dash animation is")]
        private float dashTime = 0.25f;

        [SerializeField] [Tooltip("How far does the dash reach")]
        private float dashDistance = 2f;

        [Header("Dash i-frames")]
        [SerializeField, Range(0, 1), Tooltip("In what percent of dash animation i-frames start")]
        private float iframesStart;

        [SerializeField] [Range(0, 1)] [Tooltip("In what percent of dash animation i-frames end")]
        private float iframesEnd = 0.3f;

        [Space] [SerializeField] private LayerMask groundMask;

        #endregion

        #region Properties

        public float Speed { get; set; }
        public bool CanDash { get; set; } = true;
        public bool CanMove { get; set; } = true;
        public bool CanJump { get; set; } = true;
        public bool CanRotate { get; set; } = true;
        public bool RotateSlow { get; set; } = false;
        public bool IsInvincible { get; set; }
        public bool IsGrounded { get; private set; }
        public Vector2 InputVector { get; set; }

        #endregion

        #region Cached Dependencies

        private CharacterController _controller;
        private CameraScript _camera;
        private PlayerScript _player;
        private EnemyScript _enemy;

        #endregion

        #region Private Variables

        private Vector3 _Velocity;
        private Vector3 _moveDirection;
        private float _Gravity;

        #endregion


        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _camera = CameraScript.Instance;
            _player = PlayerScript.Instance;
            _enemy = EnemyScript.Instance;
            Speed = speed;
            _Gravity = gravity;
        }

        private void FixedUpdate()
        {
            ProcessGravity();
            ProcessMovement();
        }

        private void ProcessMovement()
        {
            var vertical = InputVector.x;
            var horizontal = InputVector.y;

            _moveDirection = new Vector3(
                vertical * Mathf.Sqrt(1 - horizontal * horizontal * 0.5f),
                0,
                horizontal * Mathf.Sqrt(1 - vertical * vertical * 0.5f)
            );
            
            if (_camera.ActiveView == CameraScript.View.Player)
            {
                var desiredDirection = _enemy.transform.position - transform.position;
                var newVelocity = desiredDirection.normalized * _moveDirection.z;
                // Debug.Log("Input " + _moveDirection + " kierunek " + desiredDirection + "  velocity  " + newVelocity);
                _moveDirection = newVelocity;
            }

            if (CanMove)
                _controller.Move(_moveDirection * (Speed * Time.fixedDeltaTime));
            if (RotateSlow && !_moveDirection.Equals(Vector3.zero))
                transform.DOLookAt(transform.position + _moveDirection, 1f);
            else if (CanRotate) transform.LookAt(transform.position + _moveDirection);
        }


        public void ProcessDash()
        {
            if (!CanDash) return;

            _player.combat.InterruptCharging();
            _player.anim.Play("Dash");
            StartCoroutine(Dash(dashDistance));
        }

        public void ProcessJump()
        {
            if (!IsGrounded || !CanJump) return;

            _player.combat.Block(false);
            _player.combat.InterruptCharging();
            _player.playerInput.ResetHold();
            _player.anim.Play("Jump");
            _Velocity.y = Mathf.Sqrt(jumpHeight * -2f * _Gravity);
        }

        private void ProcessGravity()
        {
            var position = transform.position;
            IsGrounded = Physics.CheckSphere(new Vector3(position.x,
                    position.y - _controller.height / 2,
                    position.z),
                0.4f,
                groundMask);

            if (IsGrounded && _Velocity.y < 0)
                _Velocity = new Vector3(0, -2f, 0);

            _Velocity.y += _Gravity * Time.fixedDeltaTime;
            _controller.Move(_Velocity * Time.fixedDeltaTime);
        }

        public IEnumerator Dash(float distance)
        {
            _player.playerInput.ResetHold();
            _player.combat.Block(false);
            _player.combat.CanAttack = false;
            transform.DOKill();
            if (!IsGrounded)
            {
                _Gravity = 0;
                _Velocity = Vector3.zero;
            }

            CanMove = false;
            CanDash = false;
            var motion = (_moveDirection.Equals(Vector3.zero) ? transform.forward : _moveDirection.normalized) *
                         distance;
            transform.LookAt(transform.position + motion);
            var time = 0f;
            while (time < dashTime)
            {
                if (!IsInvincible && time >= iframesStart * dashTime) IsInvincible = true;
                if (IsInvincible && time >= iframesEnd * dashTime) IsInvincible = false;
                time += Time.deltaTime;
                _controller.Move(motion / (dashTime / Time.deltaTime));
                yield return new WaitForEndOfFrame();
            }

            _Gravity = gravity;
            CanMove = true;
            _player.combat.CanAttack = true;
            Invoke(nameof(ResetDashCd), dashCd);
        }


        private void ResetDashCd() => CanDash = true;
        public void ResetSpeed() => Speed = speed;

        public IEnumerator StopInAir(float time)
        {
            _Velocity = Vector3.zero;
            _Gravity /= 10;
            yield return new WaitForSeconds(time);
            _Gravity = gravity;
        }


        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            Handles.color = new Color32(10, 200, 100, 200);
            Handles.DrawWireDisc(transform.position, Vector3.up, dashDistance);
        }

        #endregion
    }
}