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
        [HideInInspector] public HitHandler hit;
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
            hit = GetComponent<HitHandler>();
            ManaBarHUD.OnRewindChange += DisableOnRewind;
        }

        public void InvokeCombo(int combo) => OnComboContinue?.Invoke(combo);

        private void DisableOnRewind(bool rewind)
        {
            anim.enabled = !rewind;
            movementScript.enabled = !rewind;
            combat.enabled = !rewind;
            hit.enabled = !rewind;
        }

        public void MechanicsOnOff(bool state)
        {
            anim.enabled = state;
            hit.enabled = state;
            combat.enabled = state;
            movementScript.enabled = state;
        }

        void OnDestroy()
        {
            ManaBarHUD.OnRewindChange -= DisableOnRewind;
        }
    }
}