using UnityEngine;

namespace Content.Scripts.Player
{
    public class PlayerCollision : MonoBehaviour
    {
        [SerializeField] private PlayerScript playerScript;

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.name != "ground") return;
            playerScript.currentGround = PlayerScript.GroundStatus.Air;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name != "ground") return;
            playerScript.currentGround = PlayerScript.GroundStatus.Ground;
        }
    }
}