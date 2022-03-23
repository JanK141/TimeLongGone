using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedAttackTrigger : MonoBehaviour
{
    [HideInInspector] public float damage;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //if(!Enemy invincable)
                DummyTest.Instance.Damage(damage);
                gameObject.SetActive(false);
        }
    }
}
