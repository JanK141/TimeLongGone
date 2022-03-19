using UnityEngine;

namespace Content.Scripts.Player
{
    public class PlayerScript : MonoBehaviour
    {
        // Store a reference to all the sub player scripts
        [SerializeField] internal PlayerInput inputScript;
        [SerializeField] internal PlayerCollision collisionScript;
        [SerializeField] internal PlayerMovement movementScript;


        // player input properties

        // move

        internal bool isLeftPressed;
        internal bool isRightPressed;
        internal bool isUpPressed;
        internal bool isDownPressed;
        internal bool isJumpPressed;
        internal bool isDashPressed;

        // combat

        internal bool isAttackPressed;
        internal bool isChargeAttackPressed;
        internal bool isBlockPressed;


        //abbility
        internal bool isTimePressed;


        // main player properties

        private HealthStatus _currentHealth;

        private enum HealthStatus
        {
            Live,
            Dead
        }


        [SerializeField] internal MoveStatus currentMove;

        internal enum MoveStatus
        {
            Stay,
            Jump,
            Fall,
            MoveUp,
            MoveDown,
            MoveLeft,
            MoveRight
        }


        [SerializeField] internal GroundStatus currentGround;

        internal enum GroundStatus
        {
            Air,
            Ground
        }

        internal CombatStatus CurrentCombat;

        internal enum CombatStatus
        {
            Idle,
            Attack,
            ChargeAttack,
            Block,
            Stun
        }

        [SerializeField] internal TimeStatus currentTime;

        internal enum TimeStatus
        {
            Continuous,
            SlowMo,
            Rewind,
            Pause,
        }


        private void Start()
        {
            CurrentCombat = CombatStatus.Idle;
            _currentHealth = HealthStatus.Live;
            currentGround = GroundStatus.Ground;
            currentTime = TimeStatus.Continuous;
            currentMove = MoveStatus.Stay;
        }
    }
}