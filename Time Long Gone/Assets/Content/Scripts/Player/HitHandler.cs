using System;
using Content.Scripts.Enemy;
using Content.Scripts.Mechanics;
using UnityEngine;

namespace Content.Scripts.Player
{
    public class HitHandler : MonoBehaviour
    {
        [SerializeField] private float parryWindow = 0.5f;
        [SerializeField] private float noCollisionTime = 0.2f;

        private PlayerScript _player;

        private void Start() => _player = PlayerScript.Instance;

        public void ReceiveHit()
        {
            print("HIT " + EnemyStatusScript.CurrStatus);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("EnemyWeapon"), true);
            Invoke(nameof(ResetCollision), noCollisionTime);
            switch (EnemyStatusScript.CurrStatus)
            {
                case Statuses.Regular:
                    if (_player.movementScript.IsInvincible) return;
                    if (_player.combat.IsBlocking)
                        if (Time.time - _player.combat.BlockPressTime <= parryWindow)
                        {
                            EnemyScript.Instance.ReceiveParry();
                            return;
                        }

                    break;
                case Statuses.Unblockable:
                    if (_player.movementScript.IsInvincible) return;
                    if (_player.combat.IsBlocking && (Time.time - _player.combat.BlockPressTime <= parryWindow))
                    {
                        EnemyScript.Instance.ReceiveParry();
                        return;
                    }

                    break;
                case Statuses.Unavoidable:
                    break;
                case Statuses.Stunned:
                    break;
                case Statuses.Vulnerable:
                    break;
                case Statuses.Invulnerable:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //Death

            StartCoroutine(Controller.Instance.PlayerDead());
            _player.IsAlive = false;
        }

        private void ResetCollision() => Physics.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("EnemyWeapon"), false
        );
    }
}