using Content.Scripts.Inputs;
using UnityEngine;

namespace Content.Scripts.Player
{
    public class PlayerScript : MonoBehaviour
    {
        // Store a reference to all the sub player scripts
        [SerializeField] internal PlayerMovement movementScript;
        [SerializeField] internal PlayerInput playerInput;
    }
}