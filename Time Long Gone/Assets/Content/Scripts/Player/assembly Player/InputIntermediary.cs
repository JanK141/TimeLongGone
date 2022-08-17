using UnityEngine;
using UnityEngine.InputSystem;

namespace Content.Scripts.Player.assembly_Player
{
    public class InputIntermediary : MonoBehaviour
    {
        [SerializeField] private float charge;

        
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


        void ProcessMove(InputAction.CallbackContext ctx)
        {
        }

        void ProcessAttack(InputAction.CallbackContext ctx)
        {
        }

        void ProcessJump(InputAction.CallbackContext ctx)
        {
        }

        void ProcessStun(InputAction.CallbackContext ctx)
        {
        }

        void ProcessBlock(InputAction.CallbackContext ctx)
        {
        }

        void ProcessFinisher(InputAction.CallbackContext ctx)
        {
        }

        void ProcessDash(InputAction.CallbackContext ctx)
        {
        }

        void ProcessTime(InputAction.CallbackContext ctx)
        {
        }

        void ProcessPause(InputAction.CallbackContext ctx)
        {
        }
    }
}