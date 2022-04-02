using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        #region Inspector Fields
        [Header("Basic movement")] 
        [SerializeField] private float speed = 10;
        [SerializeField] private float gravity = -30;
        [SerializeField] private float jumpHeight = 1;
        
        [Header("Dashing")] 
        [SerializeField] private float dashCd = 1f;
        [SerializeField] [Tooltip("How long dash animation is")] private float dashTime = 0.25f;
        [SerializeField] [Tooltip("How far does the dash reach")] private float dashDistance = 2f;

        [Header("Dash i-frames")] 
        [SerializeField, Range(0, 1), Tooltip("In what percent of dash animation i-frames start")] private float iframesStart;
        [SerializeField] [Range(0, 1)] [Tooltip("In what percent of dash animation i-frames end")] private float iframesEnd = 0.3f;

        [Space] 
        
        [SerializeField] private LayerMask groundMask;
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
        private CharacterController _Controller;
        private PlayerScript player;
        #endregion

        #region Private Variables
        private Vector3 _Velocity;
        private Vector3 _Move;
        #endregion

        private void Start()
        {
            _Controller = GetComponent<CharacterController>();
            player = PlayerScript.Instance;
            Speed = speed;
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

            _Move = new Vector3(vertical * Mathf.Sqrt(1 - horizontal * horizontal * 0.5f), 0,
                horizontal * Mathf.Sqrt(1 - vertical * vertical * 0.5f));

            if(CanMove) _Controller.Move(_Move * (Speed * Time.fixedDeltaTime));
            if (RotateSlow && !_Move.Equals(Vector3.zero)) transform.DOLookAt(transform.position + _Move, 1f);
            else if (CanRotate) transform.LookAt(transform.position + _Move);
             
        }

        public void ProcessDash()
        {
            if (!CanDash) return;

            CanDash = false;
            player.combat.IsCharging = false;
            player.anim.Play("Dash");
            StartCoroutine(Dash(dashDistance));
            Invoke(nameof(ResetDashCd), dashCd);
        }

        public void ProcessJump()
        {
            if (!IsGrounded || !CanJump) return;

            player.combat.Block(false);
            player.combat.IsCharging = false;
            player.playerInput.ResetHold();
            player.anim.Play("Jump");
            _Velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        private void ProcessGravity()
        {
            var position = transform.position;
            IsGrounded = Physics.CheckSphere(new Vector3(position.x,
                    position.y - _Controller.height / 2,
                    position.z),
                0.4f,
                groundMask);

            if (IsGrounded && _Velocity.y < 0)
                _Velocity = new Vector3(0, -2f, 0);

            _Velocity.y += gravity * Time.fixedDeltaTime;
            _Controller.Move(_Velocity * Time.fixedDeltaTime);
        }

        public IEnumerator Dash(float distance)
        {
            player.playerInput.ResetHold();
            player.combat.Block(false);
            transform.DOKill();
            float tmpGravity = gravity;
            if (!IsGrounded) gravity = 0;
            CanMove = false;
            var motion = (_Move.Equals(Vector3.zero)?transform.forward:_Move.normalized) * distance;
            transform.LookAt(transform.position+motion);
            var time = 0f;
            while (time < dashTime)
            {
                if (!IsInvincible && time >= iframesStart * dashTime) IsInvincible = true;
                if (IsInvincible && time >= iframesEnd * dashTime) IsInvincible = false;
                time += Time.deltaTime;
                _Controller.Move(motion/ (dashTime / Time.deltaTime));
                yield return new WaitForEndOfFrame();
            }

            gravity = tmpGravity;
            CanMove = true;
        }


        private void ResetDashCd() => CanDash = true;
        public void ResetSpeed() => Speed = speed;



        #region Gizmos
        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = new Color32(10, 200, 100, 200);
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, dashDistance);
        }
        #endregion
    }
}
