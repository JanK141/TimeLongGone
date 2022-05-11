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
        public static event Action OnDeath;

        void Awake()
        {
            if (Instance == null) Instance = this;
            movementScript = GetComponent<PlayerMovement>();
            playerInput = GetComponent<PlayerInput>();
            combat = GetComponent<PlayerCombat>();
            hit = GetComponent<HitHandler>();
            ManaBarHUD.OnRewindChange += DisableOnRewind;
        }
        void OnDestroy() => ManaBarHUD.OnRewindChange -= DisableOnRewind;

        public void InvokeCombo(int combo) => OnComboContinue?.Invoke(combo);
        public void InvokeDeath() => OnDeath?.Invoke();

        void DisableOnRewind(bool rewinding)
        {
            movementScript.enabled = !rewinding;
            combat.enabled = !rewinding;
            hit.enabled = !rewinding;
            anim.enabled = !rewinding;
        }
    }
}