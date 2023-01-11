using Content.Scripts.Variables;
using DG.Tweening;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy {
    public class Enemy1AnimFunctions : MonoBehaviour
    {
        [SerializeField] Enemy1 enemy;
        [SerializeField] AnimationCurve moveCurve;
        private NavMeshAgent agent;
        private Player.Player player;
        private BoolVariable IsRewinding;

        private void Start()
        {
            IsRewinding = GameLogic.Instance.IsRewinding;
            player = FindObjectOfType<Player.Player>();
            agent = enemy.GetComponent<NavMeshAgent>();
        }

        public void LookAtPlayer()
        {
            if (IsRewinding.Value) return;
            enemy.transform.DOLookAt(new Vector3(player.transform.position.x, enemy.transform.position.y, player.transform.position.z), 0.5f);
        }

        public void Move()
        {
            if (IsRewinding.Value) return;
            StartCoroutine(MoveCorutine());
        }

        private IEnumerator MoveCorutine()
        {
            float distance = Vector3.Distance(agent.transform.position, player.transform.position);
            float time = 0;
            while(time < 0.5f)
            {
                agent.Move(agent.transform.forward * Mathf.Sqrt(distance) * Time.deltaTime);
                time+=Time.deltaTime;
                yield return null;
            }
        }
    }
}
