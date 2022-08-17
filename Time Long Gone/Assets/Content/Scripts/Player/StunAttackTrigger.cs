using Content.Scripts.Enemy;
using UnityEngine;

namespace Content.Scripts.Player
{
    public class StunAttackTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;
            
            EnemyScript.Instance.ReceiveStun();
            gameObject.SetActive(false);
            PlayerScript.Instance.combat.ContinueCombo(0);
        }
    }
}
