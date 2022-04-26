using System;
using Content.Scripts.Inputs;
using UnityEngine;

namespace Content.Scripts.Player
{
    [SelectionBase]
    public class PlayerScript : MonoBehaviour
    {
        public static PlayerScript Instance;

        [SerializeField] public Animator anim;
        [HideInInspector] public PlayerMovement movementScript;
        [HideInInspector] public PlayerInput playerInput;
        [HideInInspector] public PlayerCombat combat;
        //TODO dodaæ skrypt z man¹

        public bool IsAlive { get; set; } = true;


        public static event Action<int> OnComboContinue;

        void Awake()
        {
            if (Instance == null) Instance = this;
            movementScript = GetComponent<PlayerMovement>();
            playerInput = GetComponent<PlayerInput>();
            //playerInput.SwitchCurrentActionMap("Player");
            combat = GetComponent<PlayerCombat>();
        }

        public void InvokeCombo(int combo) => OnComboContinue?.Invoke(combo);
    }
}