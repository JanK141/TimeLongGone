using Content.Scripts.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMoOve : MonoBehaviour
    {
        [SerializeField] private Camera cam; //for testing
        [SerializeField] private LayerMask groundMask; //for testing

        private NavMeshAgent _agent;
        private Animator _anim;

        private bool _isTesting; //for testing


        private bool _isWalking;

        public bool isWalking => _isWalking;

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
        private void Start() => _anim = GetComponent<Animator>();

        private void Update()
        {
            WalkTo(PlayerScript.Instance.transform.position);


            /*
            // -----------FOR TESTING ONLY --------------
            if (Input.GetKeyDown(KeyCode.T)) _isTesting = !_isTesting;
            if (_isTesting && Input.GetMouseButtonDown(0))
            {
                var ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 1000, groundMask))
                    WalkTo(hit.point);
            }
            // -----------------------------------------------
            */

            if (!_isWalking ||
                !(Vector3.Distance(transform.position, currDestination) <= _agent.stoppingDistance + 0.5f))
                return;

            _isWalking = false;
            _agent.isStopped = true;
            _agent.velocity /= 2;
            _anim.SetTrigger(StopWalking);
        }

        public void WalkTo(Vector3 destination)
        {
            _agent.isStopped = false;
            _isWalking = true;
            currDestination = destination;
            _agent.SetDestination(destination);
        }

        void SetDestination(Vector3 desVector3)
            => _agent.SetDestination(desVector3);
    }
}