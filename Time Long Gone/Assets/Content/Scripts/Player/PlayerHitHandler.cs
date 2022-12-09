using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Enemy;
using Content.Scripts.Variables;
using Player.States;
using UnityEngine;
using static Player.InputIntermediary;

namespace Player
{
    public class PlayerHitHandler : MonoBehaviour
    {
        private BoolVariable IsRewinding;
        private FloatVariable TimeToRemember;
        private FloatVariable TimeBetweenEntries;

        private Player player;
        private PlayerVariables variables;
        private PlayerTimeControl control;
        private bool ignoreHit = false;
        private LinkedList<TimeEntry> timeEntries = new LinkedList<TimeEntry>();
        private int maxentries;
        private int entries;

        [SerializeField] private AnimationCurve pushCurve;

        private void Awake()
        {
            IsRewinding = GameLogic.Instance.IsRewinding;
            TimeToRemember = GameLogic.Instance.TimeToRemember;
            TimeBetweenEntries = GameLogic.Instance.TimeBetweenEntries;
        }
        private void Start()
        {
            entries = 0;
            maxentries = (int)(TimeToRemember.Value / TimeBetweenEntries.Value);
            player = GetComponent<Player>();
            control = GetComponent<PlayerTimeControl>();
            variables = player.variables;
            StartCoroutine(Cycle());
        }

        public void ProcessHit(Enemy.AttackStatus status, Collider weaponHitBox, float pushFactor)
        {
            if (IsRewinding.Value) return;
            print("HIT by " + status.ToString());
            if (ignoreHit) return;
            switch (status)
            {
                case Enemy.AttackStatus.Regular:
                    if (player.IsBlocking)
                    {
                        if(Time.time - player.BlockTime < variables.parryWindow)
                        {
                            print("Parried");
                            control.Mana += variables.manaReward;
                            player.combat.enemy.ReceiveParry();
                            StartCoroutine(NoCollision(weaponHitBox));
                            StartCoroutine(PushPlayer(weaponHitBox.transform, pushFactor/3));
                            player.combat.ContinueCombo(0);
                            return;
                        }
                        else
                        {
                            print("Blocked");
                            StartCoroutine(NoCollision(weaponHitBox));
                            StartCoroutine(PushPlayer(weaponHitBox.transform, pushFactor/2));
                            player.combat.ContinueCombo(0);
                            return;
                        }
                    }else if (player.IsInvincible)
                    {
                        print("Dodged");
                        control.Mana += variables.manaReward;
                        StartCoroutine(NoCollision(weaponHitBox));
                        player.combat.ContinueCombo(0);
                        return;
                    }
                    break;
                case Enemy.AttackStatus.Sequence:
                    if (player.IsBlocking)
                    {
                        if (Time.time - player.BlockTime < variables.parryWindow)
                        {
                            print("Parried");
                            player.combat.enemy.ReceiveParry();
                            control.Mana += variables.manaReward;
                            StartCoroutine(NoCollision(weaponHitBox));
                            StartCoroutine(PushPlayer(weaponHitBox.transform, pushFactor / 3));
                            player.combat.ContinueCombo(0);
                            return;
                        }
                        else
                        {
                            print("Blocked");
                            StartCoroutine(NoCollision(weaponHitBox));
                            StartCoroutine(PushPlayer(weaponHitBox.transform, pushFactor / 2));
                            player.combat.ContinueCombo(0);
                            return;
                        }
                    }
                    else if (player.IsInvincible)
                    {
                        print("Dodged");
                        control.Mana += variables.manaReward;
                        StartCoroutine(NoCollision(weaponHitBox));
                        player.combat.ContinueCombo(0);
                        return;
                    }
                    break;
                case Enemy.AttackStatus.Unblockable:
                    if (player.IsBlocking)
                    {
                        if (Time.time - player.BlockTime < variables.parryWindow)
                        {
                            print("Parried");
                            player.combat.enemy.ReceiveParry();
                            control.Mana += variables.manaReward;
                            StartCoroutine(NoCollision(weaponHitBox));
                            StartCoroutine(PushPlayer(weaponHitBox.transform, pushFactor / 3));
                            player.combat.ContinueCombo(0);
                            return;
                        }
                    }
                    else if (player.IsInvincible)
                    {
                        print("Dodged");
                        control.Mana += variables.manaReward;
                        StartCoroutine(NoCollision(weaponHitBox));
                        player.combat.ContinueCombo(0);
                        return;
                    }
                    break;
                case Enemy.AttackStatus.Force:
                    StartCoroutine(PushPlayer(weaponHitBox.transform, pushFactor));
                    StartCoroutine(NoCollision(weaponHitBox));
                    return;
            }
            print("Death");
            player.combat.ContinueCombo(-1);
            StartCoroutine(PushPlayer(weaponHitBox.transform, pushFactor));
            StartCoroutine(NoCollision(weaponHitBox));
            player.CurrentState.OnStateExit();
            player.CurrentState = player.DEAD_STATE;
            player.CurrentState.OnStateEnter(true);
        }

        IEnumerator NoCollision(Collider weaponHitBox)
        {
            if (player.combat.enemy != null) Physics.IgnoreCollision(player.GetComponent<Collider>(), weaponHitBox, true);
            yield return new WaitForSeconds(variables.postHitNoCollision);
            if (player.combat.enemy != null) Physics.IgnoreCollision(player.GetComponent<Collider>(), weaponHitBox, false);
        }

        public bool isPushing = false;
        private Transform pushSource;
        private float pushFactor;
        private float time;

        IEnumerator PushPlayer(Transform source, float factor, float timeToStart = 0)
        {
            isPushing = true;
            pushSource = source;
            pushFactor = factor;
            var direction = transform.position - source.position;
            direction.y = 0f;
            var y = variables.pushVelocity * factor;
            var initVel = variables.pushVelocity * factor;
            time = timeToStart;
            while(time < variables.pushTime)
            {
                player.velocity += pushCurve.Evaluate(time/variables.pushTime) * direction * initVel;
                if(time<variables.pushTime/6)player.velocity.y = pushCurve.Evaluate(time / variables.pushTime) * y;
                time += Time.deltaTime;
                yield return null;
            }
            isPushing = false;
        }
        private void ResetIgnore() => ignoreHit = false;
        void OnEnable() => IsRewinding.OnValueChange += HandleRewind;
        void OnDisable() => IsRewinding.OnValueChange -= HandleRewind;
        private void HandleRewind()
        {
            if (IsRewinding.Value)
            {
                StopCoroutine(PushPlayer(pushSource, pushFactor));
                isPushing = false;
            }
            else
            {
                ignoreHit = true;
                Invoke(nameof(ResetIgnore), variables.postHitNoCollision);
                var entry = timeEntries.Last.Value;
                if (entry.isPushing)
                {
                    StartCoroutine(PushPlayer(entry.pushSource, entry.pushFactor, entry.pushTime));
                }
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
                        timeEntries.RemoveLast();
                        entries--;
                    }
                    yield return waitBetween;
                }
                else
                {
                    if (isPushing) timeEntries.AddLast(new TimeEntry(true, pushSource, pushFactor, time));
                    else timeEntries.AddLast(new TimeEntry(false));
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
            public bool isPushing;
            public Transform pushSource;
            public float pushFactor;
            public float pushTime;

            public TimeEntry(bool isPushing, Transform pushSource, float pushFactor, float pushTime)
            {
                this.isPushing = isPushing;
                this.pushSource = pushSource;
                this.pushFactor = pushFactor;
                this.pushTime = pushTime;
            }
            public TimeEntry(bool isPushing)
            {
                this.isPushing=isPushing;
                pushSource = null;
                pushFactor = 0;
                pushTime = 0;
            }
        }
    }
}