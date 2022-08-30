using System;
using DG.Tweening;
using Player.States;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Main player class. Works as a state machine and holds everything that other classes in composition may need.
    /// </summary>
    [SelectionBase]
    public class Player : MonoBehaviour
    {
        [SerializeField] internal PlayerVariables variables;
        [SerializeField] private LayerMask ground;
        [SerializeField] internal LayerMask enemy;

        #region Dependencies

        internal ChargedAttackHitbox chargedAttack;
        internal StunAttackHitBox stunAttack;
        internal PlayerCombat combat;
        private CharacterController controller;
        [SerializeField] internal Animator animator;

        #endregion

        #region States

        internal IPlayerState IDLE_STATE;
        internal IPlayerState JUMP_STATE;
        internal IPlayerState DASH_STATE;
        internal IPlayerState DASHATTACK_STATE;
        internal IPlayerState BLOCK_STATE;
        internal IPlayerState FINISHER_STATE;
        internal IPlayerState DEAD_STATE;
        internal IPlayerState ATTACK_STATE;
        internal IPlayerState CHARGING_STATE;
        internal IPlayerState STUN_STATE;

        #endregion

        #region Properties

        public IPlayerState CurrentState { get; set; }
        public bool CanBlock { get; set; } = true;
        public bool CanDash { get; set; } = true;
        public bool CanAttack { get; set; } = true;
        public float ChargeAttackMultiplier { get; set; } = 0;
        public bool IsBlocking { get; set; } = false;
        public float BlockTime { get; set; } = 0;
        public bool IsInvincible { get; set; } = false;
        public bool IsGrounded { get; set; } = true;
        public float Gravity { get; set; }
        /// <summary>
        /// General speed boost depending on current time scale. The lower the time scale is, the higher SpeedFactor gets.
        /// </summary>
        public float SpeedFactor { get; set; } = 1;


        #endregion

        #region Internal

        internal Vector3 velocity = Vector3.zero;
        internal Vector2 inputVector = Vector2.zero;
        public InputIntermediary.InputContext inputContext = InputIntermediary.InputContext.Nothing;
        internal Action move;
        internal Action rotate;

        #endregion

        internal Vector3 _moveDirection;
        private Camera _cam;

        void Start()
        {
            chargedAttack = GetComponentInChildren<ChargedAttackHitbox>(true);
            stunAttack = GetComponentInChildren<StunAttackHitBox>(true);
            controller = GetComponent<CharacterController>();
            combat = GetComponent<PlayerCombat>();

            move = MoveNormal;
            rotate = InstaRotate;

            IDLE_STATE = new Idle() {player = this};
            JUMP_STATE = new Jump() {jumpHeight = variables.jumpHeight, player = this};
            DASH_STATE = new Dash(){dashTime = variables.dashTime, iframesTime = variables.iframesTime, distance = variables.dashDistance, player = this};
            DASHATTACK_STATE = new DashAttack() { dashTime = variables.dashTime, iframesTime = variables.iframesTime, distance = variables.dashDistance, 
                maxDistance = variables.dashAttackMaxDist, hitBox = chargedAttack, enemy = enemy, player = this};
            BLOCK_STATE = new Block() {player = this};
            FINISHER_STATE = new Finisher() {player = this};
            DEAD_STATE = new Dead() {player = this};
            ATTACK_STATE = new Attack(){airborne = variables.airborneTime, chainTime = variables.chainTime, player = this};
            CHARGING_STATE = new Charging() {player = this};
            STUN_STATE = new Stun() {hitBox = stunAttack, player = this};

            CurrentState = IDLE_STATE;
            Gravity = variables.initialGravity;

            _cam = Camera.main;
        }

        #region Game Loop

        void Update()
        {
            SpeedFactor = (CurrentState==DEAD_STATE)? 1 : 1 + (1 - Time.timeScale) * variables.speedBoost;
            _moveDirection = CalculateMoveDirection(inputVector);
            rotate();
            move();
            CurrentState.Tick();
            var state = CurrentState.Evaluate();
            if (state != null)
            {
                CurrentState.OnStateExit();
                CurrentState = state;
                CurrentState.OnStateEnter();
            }
            UpdateAnimator();
        }

        void LateUpdate()
        {
            controller.Move(velocity * Time.deltaTime);
        }

        #endregion

        #region Move&Rotate

        internal void InstaRotate()
        {
            transform.LookAt(transform.position + _moveDirection);
        }

        internal void SlowRotate()
        {
            transform.DOLookAt(transform.position + (_moveDirection==Vector3.zero ? transform.forward : _moveDirection), variables.rotationTime / SpeedFactor);
        }

        internal void MoveNormal()
        {
            MoveGravityOnly();
            velocity += _moveDirection * variables.normalSpeed * SpeedFactor;
        }

        internal void MoveSlow()
        {
            MoveGravityOnly();
            velocity += _moveDirection * variables.slowSpeed * SpeedFactor;
        }

        internal void MoveGravityOnly()
        {
            velocity = new Vector3(0, velocity.y, 0);
            IsGrounded =
                Physics.CheckSphere(
                    new Vector3(transform.position.x, transform.position.y - controller.height / 2,
                        transform.position.z), 0.4f, ground);
            if (IsGrounded && velocity.y < 0) velocity.y = -2f;

            velocity.y += Gravity * Time.deltaTime * SpeedFactor;
        }

        #endregion

        #region Resets

        internal void ResetBlock()
        {
            CanBlock = false;
            Invoke(nameof(ResetBlockInvoke), variables.blockCooldown / SpeedFactor);
        }
        void ResetBlockInvoke() => CanBlock = true;
        internal void ResetDash()
        {
            CanDash = false;
            Invoke(nameof(ResetDashInvoke), variables.dashCooldown / SpeedFactor);
        }
        void ResetDashInvoke() => CanDash = true;
        internal void ResetAttack()
        {
            CanAttack = false;
            Invoke(nameof(ResetAttackInvoke), variables.attackCooldown / SpeedFactor);
        }
        void ResetAttackInvoke() => CanAttack = true;
        #endregion

        #region Helper methods

        /// <summary>
        /// This method maps 2D input vector into 3D vector representing move direction in reference to camera. 
        /// </summary>
        /// <param name="input">Input vector from keyboard or game pad stick</param>
        /// <returns>3D vector representing move direction in world space</returns>
        private Vector3 CalculateMoveDirection(Vector2 input)
        {
            var vertical = input.x;
            var horizontal = input.y;

            Vector3 dir = new Vector3(
                vertical * Mathf.Sqrt(1 - horizontal * horizontal * 0.5f),
                0,
                horizontal * Mathf.Sqrt(1 - vertical * vertical * 0.5f)
            );
            
            var camPos = _cam.transform.forward;

            dir =
                Quaternion.AngleAxis(
                    Vector3.SignedAngle(Vector3.forward,
                        camPos,
                        Vector3.up), Vector3.up) * dir;
            return dir;
        }
        void UpdateAnimator()
        {
            animator.speed = SpeedFactor;
            animator.SetBool("isBlocking", IsBlocking);
            animator.SetBool("isGrounded", IsGrounded);
            animator.SetFloat("verticalVelocity", velocity.y);
            animator.SetFloat("horizontalVelocity", Mathf.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z));
        }

        #endregion
    }
}