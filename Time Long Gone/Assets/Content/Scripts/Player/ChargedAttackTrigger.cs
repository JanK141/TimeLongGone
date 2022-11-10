using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Enemy;
using Content.Scripts.Player;
using UnityEngine;

public class ChargedAttackTrigger : MonoBehaviour
{
    [HideInInspector] public int damage;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyScript.Instance.ReceiveHit(damage);
            gameObject.SetActive(false);
            PlayerScript.Instance.combat.ContinueCombo(1);
        }
    }
}
