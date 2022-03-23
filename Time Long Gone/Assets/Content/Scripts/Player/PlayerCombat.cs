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
    //[SerializeField] private float attackCooldown = 0.2f;

    [Header("Charged attack")]

    [SerializeField] private Collider ChargedAttackHitBox;
    [SerializeField] private float minClampPower = 1.5f;
    [SerializeField] private float maxClampPower = 3f;

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
        if (Physics.CheckSphere(transform.position + transform.forward * attackDistance, attackRadius, enemyMask))
        {
            DummyTest.Instance.Damage(damage);
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

        chargedHitBox.gameObject.SetActive(true);
        chargedHitBox.damage = damage * strength;
        Physics.IgnoreCollision(controller, DummyTest.Instance.GetComponent<Collider>(), true);
        pm.CanDash = false;
        pm.CanMove = false;
        Vector3 motion = transform.forward;
        float time = 0f;

        while (time < player.movementScript.DashTime)
        {
            if (!pm.IsInvincible && time >= pm.FramesStart * pm.DashTime) pm.IsInvincible = true;
            if (pm.IsInvincible && time >= pm.FramesEnd * pm.DashTime) pm.IsInvincible = false;
            time += Time.deltaTime;
            controller.Move(motion * pm.Speed * strength * Time.deltaTime);
            yield return null;
        }

        pm.CanDash = true;
        pm.CanMove = true;
        Physics.IgnoreCollision(controller, DummyTest.Instance.GetComponent<Collider>(), false);
        chargedHitBox.gameObject.SetActive(false);
    }
}
