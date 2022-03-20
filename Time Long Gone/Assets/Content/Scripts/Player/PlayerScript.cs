using UnityEngine;
using UnityEngine.Serialization;

namespace Content.Scripts.Player
{
    public class PlayerScript : MonoBehaviour
    {
        // Store a reference to all the sub player scripts
        [FormerlySerializedAs("inputScript")]
        [SerializeField] internal PlayerMovement movementScript;
        
    }
}