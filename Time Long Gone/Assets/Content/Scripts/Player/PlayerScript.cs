using Content.Scripts.Inputs;
using UnityEngine;

namespace Content.Scripts.Player
{
    public class PlayerScript : MonoBehaviour
    {
        public static PlayerScript Instance;
        // Store a reference to all the sub player scripts
        [HideInInspector] public PlayerMovement movementScript;
        [HideInInspector] public PlayerInput playerInput;
        [HideInInspector] public PlayerCombat combat;

        void Awake()
        {
            if (Instance == null) Instance = this;
            movementScript = GetComponent<PlayerMovement>();
            playerInput = GetComponent<PlayerInput>();
            combat = GetComponent<PlayerCombat>();
        }
    }
}