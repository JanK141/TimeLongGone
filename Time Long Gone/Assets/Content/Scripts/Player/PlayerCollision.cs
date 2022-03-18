using UnityEngine;

namespace Content.Scripts.Player
{
    public class PlayerCollision : MonoBehaviour
    {
        [SerializeField] private PlayerMain playerScript;

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.name != "ground") return;
            playerScript.inTheAir = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name != "ground") return;
            playerScript.inTheAir = false;
        }
    }
}