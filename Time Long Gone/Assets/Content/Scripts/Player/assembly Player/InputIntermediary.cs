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
        [SerializeField] BoolVariable IsRewinding;

        private Player player;

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
        }

        void Start()
        {
            player = GetComponent<Player>();
        }

        void Update()
        {
            if (_isCharging)
            {
                _holdTime += Time.unscaledDeltaTime;
                if (_holdTime >= chargeTreshhold && player.inputContext != InputContext.ChargeStarted) player.inputContext = InputContext.ChargeStarted;
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
                IsRewinding.Value = true;
                StartCoroutine(SpeedUp());
            }
            else
            {
                float tmp = FindObjectOfType<Enemy1>().RewindTimeNeeded();
                if (tmp == 0)
                {
                    IsRewinding.Value = false;
                    StartCoroutine(SlowDown());
                }
                else
                {
                    print("Need more time - " + tmp);
                    Invoke(nameof(SetIsRewinding), tmp);
                }
            }
        }

        public void ProcessPause(InputAction.CallbackContext ctx)
        {
        }

        void SetIsRewinding() { IsRewinding.Value = false; StartCoroutine(SlowDown()); }
        IEnumerator SlowDown()
        {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(0.5f);
            float time = 0;
            while(time < 1f)
            {
                time += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(0, 1, (time / 1f));
                yield return null;
            }
            Time.timeScale = 1;
        }
        IEnumerator SpeedUp()
        {
            Time.timeScale = 0.5f;
            yield return new WaitForSecondsRealtime(0.5f);
            float time = 0;
            while (time < 1f)
            {
                time += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(0.5f, 1.5f, (time / 1f));
                yield return null;
            }
            Time.timeScale = 1.5f;
        }
    }
}