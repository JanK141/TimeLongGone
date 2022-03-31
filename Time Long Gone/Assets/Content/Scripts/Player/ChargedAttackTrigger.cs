using UnityEngine;

public class ChargedAttackTrigger : MonoBehaviour
{
    [HideInInspector] public float damage;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        
        //if(!Enemy invincable)
        
        DummyTest.Instance.Damage(damage);
        gameObject.SetActive(false);
        
    }
}
