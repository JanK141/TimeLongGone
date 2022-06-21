using Content.Scripts.Player;
using UnityEngine;

namespace Content.Scripts.Enemy
{
    public class WeaponHitBox : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.layer.Equals(LayerMask.NameToLayer("Player"))) return;
        
            PlayerScript.Instance.hit.ReceiveHit();
            GetComponent<Collider>().enabled = false;
        }

    }
}
