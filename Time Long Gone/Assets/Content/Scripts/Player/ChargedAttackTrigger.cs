using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using UnityEngine;

public class ChargedAttackTrigger : MonoBehaviour
{
    [HideInInspector] public int damage;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //if(!Enemy invincable)
                DummyTest.Instance.Damage(damage);
                gameObject.SetActive(false);
                PlayerScript.Instance.combat.ContinueCombo(1);
        }
    }
}
