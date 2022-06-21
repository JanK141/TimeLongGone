using System.Collections;
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
        public bool canDash { get; set; } = true;
        public bool canMove { get; set; } = true;
        public bool canJump { get; set; } = true;
        public bool canRotate { get; set; } = true;
        public bool rotateSlow { get; set; }
        public bool isInvincible { get; set; }
        public bool isGrounded { get; private set; }
        public Vector2 inputVector { get; set; }

        #endregion

        #region Cached Dependencies

        private CharacterController _controller;
        //private CameraScript _camera;
        private PlayerScript _player;
        private EnemyScript _enemy;

        #endregion

        #region Private Variables

        private Vector3 _velocity;
        private Vector3 _moveDirection;
        private float _gravity;
        private UnityEngine.Camera _cam;

        #endregion


        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            //_camera = CameraScript.Instance;
            _player = PlayerScript.Instance;
            _enemy = EnemyScript.Instance;
            Speed = speed;
            _gravity = gravity;
            _cam = UnityEngine.Camera.main;
        }

        private void FixedUpdate()
        {
            ProcessGravity();
            ProcessMovement();
        }

        private void ProcessMovement()
        {
            var vertical = inputVector.x;
            var horizontal = inputVector.y;

            _moveDirection = new Vector3(
                vertical * Mathf.Sqrt(1 - horizontal * horizontal * 0.5f),
                0,
                horizontal * Mathf.Sqrt(1 - vertical * vertical * 0.5f)
            );

            var camPos = _cam.transform.forward;

            _moveDirection =
                Quaternion.AngleAxis(
                    Vector3.SignedAngle(Vector3.forward, 
                        camPos,
                        Vector3.up), Vector3.up) * _moveDirection;
       
            if (canMove)
                _controller.Move(_moveDirection * (Speed * Time.fixedDeltaTime));
            if (rotateSlow && !_moveDirection.Equals(Vector3.zero))
                transform.DOLookAt(transform.position + _moveDirection, 1f);
            else if (canRotate) transform.LookAt(transform.position + _moveDirection);
        }


        public void ProcessDash()
        {
            if (!canDash) return;

            _player.combat.InterruptCharging();
            _player.anim.Play("Dash");
            StartCoroutine(Dash(dashDistance));
        }

        public void ProcessJump()
        {
            if (!isGrounded || !canJump) return;

            _player.combat.Block(false);
            _player.combat.InterruptCharging();
            _player.playerInput.ResetHold();
            _player.anim.Play("Jump");
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * _gravity);
        }

        private void ProcessGravity()
        {
            var position = transform.position;
            isGrounded = Physics.CheckSphere(new Vector3(position.x,
                    position.y - _controller.height / 2,
                    position.z),
                0.4f,
                groundMask);

            if (isGrounded && _velocity.y < 0)
                _velocity = new Vector3(0, -2f, 0);

            _velocity.y += _gravity * Time.fixedDeltaTime;
            _controller.Move(_velocity * Time.fixedDeltaTime);
        }

        public IEnumerator Dash(float distance)
        {
            _player.playerInput.ResetHold();
            _player.combat.Block(false);
            _player.combat.CanAttack = false;
            transform.DOKill();
            if (!isGrounded)
            {
                _gravity = 0;
                _velocity = Vector3.zero;
            }

            canMove = false;
            canDash = false;
            var motion = (_moveDirection.Equals(Vector3.zero) ? transform.forward : _moveDirection.normalized) *
                         distance;
            transform.LookAt(transform.position + motion);
            var time = 0f;
            while (time < dashTime)
            {
                if (!isInvincible && time >= iframesStart * dashTime) isInvincible = true;
                if (isInvincible && time >= iframesEnd * dashTime) isInvincible = false;
                time += Time.deltaTime;
                _controller.Move(motion / (dashTime / Time.deltaTime));
                yield return new WaitForEndOfFrame();
            }

            _gravity = gravity;
            canMove = true;
            _player.combat.CanAttack = true;
            Invoke(nameof(ResetDashCd), dashCd);
        }


        private void ResetDashCd() => canDash = true;
        public void ResetSpeed() => Speed = speed;

        public IEnumerator StopInAir(float time)
        {
            _velocity = Vector3.zero;
            _gravity /= 10;
            yield return new WaitForSeconds(time);
            _gravity = gravity;
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