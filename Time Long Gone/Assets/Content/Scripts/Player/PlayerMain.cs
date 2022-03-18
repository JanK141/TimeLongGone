using UnityEngine;

namespace Content.Scripts.Player
{
    public class PlayerMain : MonoBehaviour
    {
        // Store a reference to all the sub player scripts
        [SerializeField] internal PlayerInput inputScript;
        [SerializeField] internal PlayerCollision collisionScript;


        // main player properties

        [SerializeField] internal int health = 100;
        [SerializeField] internal float walkSpeed = 6f;
        [SerializeField] internal bool inTheAir;
        [SerializeField] internal bool isAttacking;
        [SerializeField] internal bool isChargingAttack;
        [SerializeField] internal bool isBlocking;

    
    }
}