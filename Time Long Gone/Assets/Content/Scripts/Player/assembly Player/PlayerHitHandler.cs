using System;
using System.Collections;
using Content.Scripts.Enemy;
using UnityEngine;

namespace Player
{
    public class PlayerHitHandler : MonoBehaviour
    {
        private Player player;
        private PlayerVariables variables;

        [SerializeField] private AnimationCurve pushCurve;

        private void Start()
        {
            player = GetComponent<Player>();
            variables = player.variables;
        }

        public void ProcessHit(Enemy.AttackStatus status, Collider weaponHitBox, float pushFactor)
        {
            print("HIT by " + status.ToString());

            switch (status)
            {
                case Enemy.AttackStatus.Regular:
                    if (player.IsBlocking)
                    {
                        if(Time.time - player.BlockTime < variables.parryWindow)
                        {
                            print("Parried");
                            StartCoroutine(NoCollision(weaponHitBox));
                            StartCoroutine(PushPlayer(weaponHitBox.transform, pushFactor/3));
                            return;
                        }
                        else
                        {
                            print("Blocked");
                            StartCoroutine(NoCollision(weaponHitBox));
                            StartCoroutine(PushPlayer(weaponHitBox.transform, pushFactor/2));
                            return;
                        }
                    }else if (player.IsInvincible)
                    {
                        print("Dodged");
                        StartCoroutine(NoCollision(weaponHitBox));
                        return;
                    }
                    break;
                case Enemy.AttackStatus.Sequence:
                    if (player.IsBlocking)
                    {
                        if (Time.time - player.BlockTime < variables.parryWindow)
                        {
                            print("Parried");
                            StartCoroutine(NoCollision(weaponHitBox));
                            StartCoroutine(PushPlayer(weaponHitBox.transform, pushFactor / 3));
                            return;
                        }
                        else
                        {
                            print("Blocked");
                            StartCoroutine(NoCollision(weaponHitBox));
                            StartCoroutine(PushPlayer(weaponHitBox.transform, pushFactor / 2));
                            return;
                        }
                    }
                    else if (player.IsInvincible)
                    {
                        print("Dodged");
                        StartCoroutine(NoCollision(weaponHitBox));
                        return;
                    }
                    break;
                case Enemy.AttackStatus.Unblockable:
                    if (player.IsBlocking)
                    {
                        if (Time.time - player.BlockTime < variables.parryWindow)
                        {
                            print("Parried");
                            StartCoroutine(NoCollision(weaponHitBox));
                            StartCoroutine(PushPlayer(weaponHitBox.transform, pushFactor / 3));
                            return;
                        }
                    }
                    else if (player.IsInvincible)
                    {
                        print("Dodged");
                        StartCoroutine(NoCollision(weaponHitBox));
                        return;
                    }
                    break;
                case Enemy.AttackStatus.Force:
                    StartCoroutine(PushPlayer(weaponHitBox.transform, pushFactor));
                    break;
            }
            print("Death");
        }

        IEnumerator NoCollision(Collider weaponHitBox)
        {
            if (player.combat.enemy != null) Physics.IgnoreCollision(player.GetComponent<Collider>(), weaponHitBox, true);
            yield return new WaitForSeconds(variables.postHitNoCollision);
            if (player.combat.enemy != null) Physics.IgnoreCollision(player.GetComponent<Collider>(), weaponHitBox, false);
        }

        IEnumerator PushPlayer(Transform source, float factor)
        {
            var direction = transform.position - source.position;
            direction.y = 0f;
            var y = variables.pushVelocity * factor;
            var initVel = variables.pushVelocity * factor;
            float time = 0f;
            while(time < variables.pushTime)
            {
                player.velocity += pushCurve.Evaluate(time/variables.pushTime) * direction * initVel;
                if(time<variables.pushTime/6)player.velocity.y = pushCurve.Evaluate(time / variables.pushTime) * y;
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}