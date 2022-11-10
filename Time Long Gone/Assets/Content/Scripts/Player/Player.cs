using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.Variables;
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
        [SerializeField] private LayerMask ground;
        [SerializeField] internal LayerMask enemy;
        [SerializeField] internal Animator animator;

        #region Dependencies

        internal BoolVariable IsRewinding;
        private FloatVariable TimeToRemember;
        private FloatVariable TimeBetweenEntries;
        internal ChargedAttackHitbox chargedAttack;
        internal StunAttackHitBox stunAttack;
        internal PlayerCombat combat;
        internal PlayerHitHandler hitHandler;
        private CharacterController controller;

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

        internal PlayerVariables variables;
        internal Vector3 velocity = Vector3.zero;
        internal Vector2 inputVector = Vector2.zero;
        public InputIntermediary.InputContext inputContext = InputIntermediary.InputContext.Nothing;
        internal Action move;
        internal Action rotate;

        #endregion

        internal Vector3 _moveDirection;
        private Camera _cam;

        private LinkedList<TimeEntry> timeEntries = new LinkedList<TimeEntry>();
        private int maxentries;
        private int entries;

        private void Awake()
        {
            IsRewinding = Resources.Load<BoolVariable>("Rewind/IsRewinding");
            TimeToRemember = Resources.Load<FloatVariable>("Rewind/TimeToRemember");
            TimeBetweenEntries = Resources.Load<FloatVariable>("Rewind/TimeBetweenEntries");
            variables = Resources.LoadAll<PlayerVariables>("PlayerVariables").FirstOrDefault();
        }
        void Start()
        {
            entries = 0;
            maxentries = (int)(TimeToRemember.Value / TimeBetweenEntries.Value);
            chargedAttack = GetComponentInChildren<ChargedAttackHitbox>(true);
            stunAttack = GetComponentInChildren<StunAttackHitBox>(true);
            controller = GetComponent<CharacterController>();
            combat = GetComponent<PlayerCombat>();
            hitHandler = GetComponent<PlayerHitHandler>();

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
            StartCoroutine(Cycle());
        }

        #region Game Loop

        void Update()
        {
            if (IsRewinding.Value || CurrentState==DEAD_STATE) return;
            SpeedFactor = (CurrentState==DEAD_STATE)? 1 : 1 + (1 - Time.timeScale) * variables.speedBoost;
            if(!hitHandler.isPushing)_moveDirection = CalculateMoveDirection(inputVector);
            rotate();
            move();
            CurrentState.Tick();
            var state = CurrentState.Evaluate();
            if (state != null)
            {
                CurrentState.OnStateExit();
                CurrentState = state;
                CurrentState.OnStateEnter(true);
            }
            UpdateAnimator();
        }

        void LateUpdate()
        {
            if(!IsRewinding.Value)controller.Move(velocity * Time.deltaTime);
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
                        Vector3.ProjectOnPlane(camPos, Vector3.up),
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

        #region Rewinding

        void OnEnable() => IsRewinding.OnValueChange += HandleRewind;
        void OnDisable() => IsRewinding.OnValueChange -= HandleRewind;
        private void HandleRewind()
        {
            if (!IsRewinding.Value)
            {
                var entry = timeEntries.Last.Value;
                CurrentState = entry.state;
                velocity = entry.velocity;
                ChargeAttackMultiplier = entry.charge;
                CurrentState.OnStateEnter(false);
                (CurrentState as Dash)?.SeTime((float)entry.dashtime);
                if (CurrentState is Finisher) inputContext = InputIntermediary.InputContext.FinisherCanceled;
                else if (CurrentState is Block) inputContext = InputIntermediary.InputContext.BlockCanceled;
                else if (CurrentState is Charging) inputContext = InputIntermediary.InputContext.ChargeCanceled;
            }
        }

        IEnumerator Cycle()
        {
            YieldInstruction waitBetween = new WaitForSeconds(TimeBetweenEntries.Value);
            while (true)
            {
                if (IsRewinding.Value)
                {
                    if (entries > 1)
                    {
                        CurrentState = timeEntries.Last.Value.state;
                        timeEntries.RemoveLast();
                        entries--;
                    }
                    yield return waitBetween;
                }
                else
                {
                    timeEntries.AddLast(new TimeEntry(CurrentState, velocity, ChargeAttackMultiplier, (CurrentState as Dash)?.GetTime()));
                    entries++;
                    if (entries > maxentries)
                    {
                        timeEntries.RemoveFirst();
                        entries--;
                    }
                    yield return waitBetween;
                }
            }
        }

        private struct TimeEntry
        {
            public IPlayerState state;
            public Vector3 velocity;
            public float charge;
            public float? dashtime;

            public TimeEntry(IPlayerState state, Vector3 velocity, float charge, float? dashtime)
            {
                this.state = state;
                this.velocity = velocity;
                this.charge = charge;
                this.dashtime = dashtime;
            }
        }

        #endregion
    }
}