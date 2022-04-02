using System.Collections;
using UnityEngine;

namespace Content.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public static PlayerMovement Instance;

        [Header("Basic movement")] [SerializeField]
        private float speed = 10;

        [SerializeField] private float gravity = -30;
        [SerializeField] private float jumpHeight = 1;
        [Header("Dashing")] [SerializeField] private float dashCd = 1f;

        [SerializeField] [Tooltip("How long dash animation is")]
        private float dashTime = 0.25f;

        [SerializeField] [Tooltip("What product of speed is applied every frame of a dash")]
        private float dashMoveMultiplier = 2f;

        [Header("Dash i-frames"), SerializeField, Range(0, 1),
         Tooltip("In what percent of dash animation i-frames start")]
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
            get => m_CanDash;
            set => m_CanDash = value;
        }

        public bool CanMove
        {
            get => m_CanMove;
            set => m_CanMove = value;
        }

        public bool isInvincible { get; set; }

        public Vector3 inputVector { get; set; }

        public Vector3 currPosition { get; set; }

        public Vector3 velocity
        {
            get => m_Velocity;
            set => m_Velocity = value;
        }

        public float Gravity => gravity;

        private Vector3 m_Velocity;
        private Vector3 m_Move;
        private bool m_IsGrounded;
        private bool m_CanDash = true;
        private bool m_CanMove = true;

        private CharacterController m_Controller;
        private PlayerScript player;

        private void Awake() => Instance = this;

        private void Start()
        {
            m_Controller = GetComponent<CharacterController>();
            player = PlayerScript.Instance;
        }

        private void Update()
        {
            ProcessGravity();
            ProcessMovement();
        }

        private void ProcessMovement()
        {
            var vertical = inputVector.x;
            var horizontal = inputVector.z;

            m_Move = new Vector3(vertical * Mathf.Sqrt(1 - horizontal * horizontal * 0.5f), 0,
                horizontal * Mathf.Sqrt(1 - vertical * vertical * 0.5f));

            if (!m_CanMove || !(m_Move.magnitude > 0.05)) return;

            m_Controller.Move(m_Move * (speed * Time.deltaTime));
            transform.LookAt(transform.position + m_Move);
        }

        public void ProcessDash()
        {
            if (!m_CanDash) return;
            m_CanDash = false;
            StartCoroutine(nameof(Dash));
            Invoke(nameof(ResetDashCd), dashCd);
        }

        public void ProcessJump()
        {
            if (!m_IsGrounded) return;
            m_Velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        private void ProcessGravity()
        {
            var position = transform.position;
            m_IsGrounded = Physics.CheckSphere(new Vector3(position.x,
                    position.y - m_Controller.height / 2,
                    position.z),
                0.4f,
                groundMask);

            if (m_IsGrounded && m_Velocity.y < 0)
                m_Velocity = new Vector3(0, -2f, 0);

            m_Velocity.y += gravity * Time.deltaTime;
            m_Controller.Move(m_Velocity * Time.deltaTime);
        }

        private IEnumerator Dash()
        {
            m_CanMove = false;
            var motion = m_Move.normalized;
            var time = 0f;
            while (time < dashTime)
            {
                if (!isInvincible && time >= iframesStart * dashTime) isInvincible = true;
                if (isInvincible && time >= iframesEnd * dashTime) isInvincible = false;
                time += Time.deltaTime;
                m_Controller.Move(motion * speed * dashMoveMultiplier * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            m_CanMove = true;
        }

        private void ResetDashCd() => m_CanDash = true;

        /*
        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = new Color32(10, 200, 100, 200);
            if (GetComponent<DrawGizmos>().drawGizmos)
                UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, speed * dashMoveMultiplier * dashTime);
        }
        */
    }
}