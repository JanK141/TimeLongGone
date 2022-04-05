using UnityEngine;

namespace Content.Scripts.Enemy
{
    public class EnemyScript : MonoBehaviour
    {
        public static EnemyScript Instance;

        // Store a reference to all the sub enemy scripts
        // [HideInInspector] public EnemyMovement movement;
        // [HideInInspector] public EnemyCombat combat;
        [HideInInspector] public EnemyHealth health;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            health = GetComponent<EnemyHealth>();
            //  movement = GetComponent<EnemyMovement>();
            //  combat = GetComponent<PlayerCombat>();
        }
    }
}