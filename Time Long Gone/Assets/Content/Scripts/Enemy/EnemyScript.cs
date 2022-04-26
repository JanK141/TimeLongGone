using System;
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

        public static event Action<int, int> OnEnemyHeatlhChange; // sends max and current health

        private void Awake()
        {
            if (Instance == null) Instance = this;
            health = GetComponent<EnemyHealth>();
            move = GetComponent<EnemyMoOve>();
            anim = GetComponentInChildren<Animator>();
        }

        public void ReceiveHit(int damage)
        {
            health.CurrHealth -= damage;
            transform.DOPunchPosition(-(PlayerScript.Instance.transform.position - transform.position).normalized * 0.2f, 0.1f);
            OnEnemyHeatlhChange?.Invoke(health.MaxHealth, health.CurrHealth);
        }
    }
}