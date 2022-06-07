﻿using System;
using Content.Scripts.Camera;
using Content.Scripts.Player;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Enemy
{
    public class EnemyScript : MonoBehaviour
    {
        public static EnemyScript Instance;

        [HideInInspector] public EnemyHealth health;
        [HideInInspector] public EnemyMoOve move;
        [HideInInspector] public Animator anim;
        [HideInInspector] public EnemyStatusScript status;

        private bool _isStunned = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            health = GetComponent<EnemyHealth>();
            move = GetComponent<EnemyMoOve>();
            anim = GetComponentInChildren<Animator>();
            status = GetComponentInChildren<EnemyStatusScript>();
            ManaBarHUD.OnRewindChange += DisableOnRewind;
        }

        public void ReceiveHit(int damage)
        {
            if (EnemyStatusScript.CurrStatus != Statuses.Invulnerable)
            {
                health.CurrHealth -= damage;
                transform.DOPunchPosition(
                    -(PlayerScript.Instance.transform.position - transform.position).normalized * 0.2f, 0.1f);
            }
        }

        public void ReceiveStun()
        {
            if (EnemyStatusScript.CurrStatus == Statuses.Vulnerable)
            {
                status.MakeEnemyStunned();
                anim.Play("StunStart");
                CinemachineSwitcher.Instance.Switch(true);
                Invoke(nameof(EndStun), 4f);
            }
        }

        void EndStun()
        {
            if (EnemyStatusScript.CurrStatus == Statuses.Stunned)
            {
                anim.Play("StunEnd");
                CinemachineSwitcher.Instance.Switch(false);
            }
        }

        public void ReceiveParry()
        {
            anim.Play("Parried");
            status.MakeEnemyVulnerable();
            Invoke(nameof(EndParry), 1f);
        }

        public void EndParry()
        {
            if (EnemyStatusScript.CurrStatus >0 && EnemyStatusScript.CurrStatus < (Statuses) 10) anim.Play("ParryEnd");
        }

        private void DisableOnRewind(bool rewind)
        {
            anim.enabled = !rewind;
            move.enabled = !rewind;
            status.enabled = !rewind;
        }

        void OnDestroy()
        {
            ManaBarHUD.OnRewindChange -= DisableOnRewind;
        }
    }
}