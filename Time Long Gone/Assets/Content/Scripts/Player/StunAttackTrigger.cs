using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using UnityEngine;

public class StunAttackTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //if(Enemy vulnerable)
            DummyTest.Instance.Stun();
            gameObject.SetActive(false);
            PlayerScript.Instance.combat.ContinueCombo(0);
        }
    }
}
