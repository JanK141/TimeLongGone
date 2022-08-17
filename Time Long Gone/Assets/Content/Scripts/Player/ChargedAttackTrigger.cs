using Content.Scripts.Enemy;
using UnityEngine;

namespace Content.Scripts.Player
{
    public class ChargedAttackTrigger : MonoBehaviour
    {
        [HideInInspector] public int damage;

        public void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;
            EnemyScript.Instance.ReceiveHit(damage);
            gameObject.SetActive(false);
            PlayerScript.Instance.combat.ContinueCombo(1);
        }
    }
}
