using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    #region Inspector Fields
    [Header("Normal attack")]

    [SerializeField] float attackRadius = 1;
    [SerializeField] private float attackDistance = 0.5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lastHitDamage = 15f;
    //[SerializeField] private float attackCooldown = 0.2f;

    [Header("Charged attack")]

    [SerializeField] private Collider ChargedAttackHitBox;
    [SerializeField] private float minClampPower = 1.5f;
    [SerializeField] private float maxClampPower = 3f;
    [SerializeField] private float maxDistance = 8f;

    [Space] [SerializeField] private LayerMask enemyMask;
    #endregion

    #region Cached dependencies
    private PlayerScript player;
    private ChargedAttackTrigger chargedHitBox;
    private CharacterController controller;
    #endregion

    #region private variables

    private bool canAttack = true;

    #endregion

    void Start()
    {
        player = PlayerScript.Instance;
        chargedHitBox = ChargedAttackHitBox.GetComponent<ChargedAttackTrigger>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        
    }

    public void Attack()
    {
        if(!canAttack) return;
        player.anim.SetTrigger("Attack");
    }

    public void Hit()
    {
        if (Physics.CheckSphere(transform.position + transform.forward * attackDistance, attackRadius, enemyMask))
        {
            DummyTest.Instance.Damage(damage);
        }
    }
    public void LastHit()
    {
        if (Physics.CheckSphere(transform.position + transform.forward * attackDistance, attackRadius, enemyMask))
        {
            DummyTest.Instance.Damage(lastHitDamage);
        }
    }

    public void ChargedAttack(float power)
    {
        if(!canAttack) return;
        StartCoroutine(DashAttack(Mathf.Clamp(power, minClampPower, maxClampPower)));
    }

    IEnumerator DashAttack(float strength)
    {
        var pm = player.movementScript;

        player.anim.SetBool("DashAttack", true);
        chargedHitBox.gameObject.SetActive(true);
        chargedHitBox.damage = damage * strength;
        Physics.IgnoreCollision(controller, DummyTest.Instance.GetComponent<Collider>(), true);
        pm.CanDash = false;

        yield return StartCoroutine(pm.Dash(Mathf.Clamp((strength-minClampPower) / (maxClampPower - minClampPower),0.5f, 1f) * maxDistance));

        pm.CanDash = true;
        Physics.IgnoreCollision(controller, DummyTest.Instance.GetComponent<Collider>(), false);
        chargedHitBox.gameObject.SetActive(false);
        player.anim.SetBool("DashAttack", false);
    }

    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = new Color32(255, 0, 0, 200);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, maxDistance);
    }
}
