using Content.Scripts.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMoOve : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private EnemyScript _enemy;


        //private bool _isWalking;

        //public bool isWalking => _isWalking;

        private Vector3 currDestination;
        private static readonly int StopWalking = Animator.StringToHash("StopWalking");

        public Vector3 CurrDestination
        {
            get => currDestination;
            set
            {
                currDestination = value;
                SetDestination(value);
            }
        }

        private void Awake() => _agent = GetComponent<NavMeshAgent>();
        private void Start() => _enemy = GetComponent<EnemyScript>();

        private void Update()
        {
            //WalkTo(PlayerScript.Instance.transform.position);
            if (!_agent.isStopped && (Vector3.Distance(transform.position, currDestination) <= _agent.stoppingDistance + 0.25f))
            {
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;
                _enemy.anim.SetTrigger(StopWalking);
            }

                // _isWalking = false;
            //_agent.isStopped = true;
           // _agent.velocity /= 2;
        }

        public void WalkTo(Vector3 destination)
        {
            _agent.isStopped = false;
            //_isWalking = true;
            currDestination = destination;
            _agent.SetDestination(destination);
        }

        void SetDestination(Vector3 desVector3)
            => _agent.SetDestination(desVector3);
    }
}