using Content.Scripts.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyWalk : MonoBehaviour
    {
        [SerializeField] private Camera cam; //for testing
        [SerializeField] private LayerMask groundMask; //for testing

        private NavMeshAgent agent;
        private Animator anim;

        private bool isTesting; //for testing


        private bool isWalking;

        public bool IsWalking => isWalking;

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

        private void Awake() => agent = GetComponent<NavMeshAgent>();
        private void Start() => anim = GetComponent<Animator>();

        private void Update()
        {
            WalkTo(PlayerMovement.Instance.transform.position);
            /*
             // -----------FOR TESTING ONLY --------------
             if (Input.GetKeyDown(KeyCode.T)) isTesting = !isTesting;
             if (isTesting && Input.GetMouseButtonDown(0))
             {
                 var ray = cam.ScreenPointToRay(Input.mousePosition);
                 RaycastHit hit;
                 if (Physics.Raycast(ray, out hit, 1000, groundMask))
                     WalkTo(hit.point);
             }
             // -----------------------------------------------
         */
            if (!isWalking ||
                !(Vector3.Distance(transform.position, currDestination) <= agent.stoppingDistance + 0.5f)) return;
            isWalking = false;
            agent.isStopped = true;
            agent.velocity /= 2;
            anim.SetTrigger(StopWalking);
        }

        internal void WalkTo(Vector3 destination)
        {
            agent.isStopped = false;
            isWalking = true;
            currDestination = destination;
            agent.SetDestination(destination);
        }

        private void SetDestination(Vector3 desVector3)
            => agent.SetDestination(desVector3);
    }
}