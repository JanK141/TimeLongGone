using Content.Scripts.Variables;
using Enemy;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputIntermediary : MonoBehaviour
    {
        [SerializeField] private float chargeTreshhold = 0.25f;

        private Player player;
        private PlayerTimeControl control;

        private bool _isCharging = false;
        private float _holdTime = 0;

        public enum InputContext
        {
            Nothing,
            Jump,
            Dash,
            Stun,
            BlockStarted,
            BlockCanceled,
            FinisherStarted,
            FinisherCanceled,
            Attack,
            ChargeStarted,
            ChargeCanceled,

            RewindStarted,
            RewindCanceled
        }

        void Start()
        {
            player = GetComponent<Player>();
            control = GetComponent<PlayerTimeControl>();
        }

        void Update()
        {
            if (_isCharging)
            {
                _holdTime += Time.unscaledDeltaTime;
                if (_holdTime >= chargeTreshhold && player.inputContext != InputContext.ChargeStarted)
                    player.inputContext = InputContext.ChargeStarted;
            }
        }

        public void ProcessMove(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                var x = ctx.ReadValue<Vector2>().x;
                var y = ctx.ReadValue<Vector2>().y;
                player.inputVector = new Vector2(x, y);
            }
            else if (ctx.canceled)
                player.inputVector = new Vector2(0, 0);
        }

        public void ProcessAttack(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                _isCharging = true;
            }
            else if ((ctx.performed || ctx.canceled) && _isCharging)
            {
                if (_holdTime >= chargeTreshhold) player.inputContext = InputContext.ChargeCanceled;
                else player.inputContext = InputContext.Attack;
                _isCharging = false;
                _holdTime = 0;
            }
        }

        public void ProcessJump(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) player.inputContext = InputContext.Jump;
        }

        public void ProcessStun(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) player.inputContext = InputContext.Stun;
        }

        public void ProcessBlock(InputAction.CallbackContext ctx)
        {
            if (ctx.started) player.inputContext = InputContext.BlockStarted;
            else if (ctx.performed || ctx.canceled) player.inputContext = InputContext.BlockCanceled;
        }

        public void ProcessFinisher(InputAction.CallbackContext ctx)
        {
            if (ctx.started) player.inputContext = InputContext.FinisherStarted;
            else if (ctx.performed || ctx.canceled) player.inputContext = InputContext.FinisherCanceled;
        }

        public void ProcessDash(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) player.inputContext = InputContext.Dash;
        }

        public void ProcessTime(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                control.WantsToTimeControl = true;
                player.inputContext = InputContext.RewindStarted;
            }
            else if (ctx.performed || ctx.canceled)
            {
                
                control.WantsToTimeControl = false;
                if (ctx.canceled)
                    player.inputContext = InputContext.RewindCanceled;
            }
        }

        public void ProcessPause(InputAction.CallbackContext ctx)
        {
            PausingScript.Instance.Pausing();
        }
    }
}