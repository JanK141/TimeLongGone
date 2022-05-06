using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Enemy;
using Content.Scripts.Player;
using UnityEngine;

public class StunAttackTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyScript.Instance.ReceiveStun();
            gameObject.SetActive(false);
            PlayerScript.Instance.combat.ContinueCombo(0);
        }
    }
}
