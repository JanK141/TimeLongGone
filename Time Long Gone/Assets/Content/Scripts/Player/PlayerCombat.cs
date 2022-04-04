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

    [Header("Block")] [SerializeField] private float blockCD = 0.5f;

    [Space] [SerializeField] private LayerMask enemyMask;
    #endregion

    #region Cached dependencies
    private PlayerScript player;
    private ChargedAttackTrigger chargedHitBox;
    private CharacterController controller;
    #endregion

    #region Properties
    public bool CanAttack { get; set; } = true;
    public float Damage { get; set; }
    public bool IsCharging { get; set; } = false;
    public bool IsBlocking { get; set; } = false;
    public bool CanBlock { get; set; } = true;
    #endregion

    void Start()
    {
        Damage = damage;
        player = PlayerScript.Instance;
        chargedHitBox = ChargedAttackHitBox.GetComponent<ChargedAttackTrigger>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        
    }

    public void Attack()
    {
        if(!CanAttack) return;

        CanAttack = false;
        player.movementScript.CanRotate = false;
        player.movementScript.CanMove = false;
        var state = player.anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Attack2post"))
        {
            player.anim.Play("Attack3");
        }
        else if (state.IsName("Attack1post"))
        {
            player.anim.Play("Attack2");
        }
        else
        {
            player.anim.Play("Attack1");
        }
    }

    public void Hit()
    {
        if (Physics.CheckSphere(transform.position + transform.forward * attackDistance, attackRadius, enemyMask))
        {
            DummyTest.Instance.Damage(Damage);
        }
    }
    public void LastHit()
    {
        if (Physics.CheckSphere(transform.position + transform.forward * attackDistance, attackRadius, enemyMask))
        {
            DummyTest.Instance.Damage(lastHitDamage);
        }
    }

    public void HeavyAttack()
    {
        if(!CanAttack || !player.movementScript.IsGrounded) return;
        
        player.anim.Play("HeavyAttack");
        StickToGround(false);
    }

    public void StunAttack()
    {
        player.anim.Play("StunAttack");
        StickToGround(false);
    }

    public void Block(bool state)
    {
        if(!CanBlock) return;

        IsBlocking = state;
        InterruptCharging();
        if (state)
        {
            player.anim.Play("Block");
            player.movementScript.Speed = 0.5f;
        }
        else
        {
            CanBlock = false;
            player.anim.SetTrigger("StopBlock");
            Invoke(nameof(ResetBlock), blockCD);
        }
    }

    public void StartCharging()
    {
        if (!CanAttack || !player.movementScript.IsGrounded) return;
        IsCharging = true;
        CanAttack = false;
        player.movementScript.Speed = 1.5f;
        player.anim.Play("Charging");
    }
    public void ChargedAttack(float power)
    {
        if(!IsCharging) return;
        player.anim.Play("DashAttack");
        InterruptCharging();
        StartCoroutine(DashAttack(Mathf.Clamp(power, minClampPower, maxClampPower)));
    }

    IEnumerator DashAttack(float strength)
    {
        chargedHitBox.gameObject.SetActive(true);
        chargedHitBox.damage = Damage * strength;
        Physics.IgnoreCollision(controller, DummyTest.Instance.GetComponent<Collider>(), true);

        yield return StartCoroutine(player.movementScript.Dash(Mathf.Clamp((strength-minClampPower) / (maxClampPower - minClampPower),0.5f, 1f) * maxDistance));

        Physics.IgnoreCollision(controller, DummyTest.Instance.GetComponent<Collider>(), false);
        chargedHitBox.gameObject.SetActive(false);
        player.movementScript.ResetSpeed();
    }

    public void InvokeAttackReset(float time) => Invoke(nameof(AttackReset), time);
    public void AttackReset()
    {
        CanAttack = true;
        player.movementScript.CanRotate = true;
        player.movementScript.CanMove = true;
    }

    public void ResetHeavy()
    {
        if (player.anim.GetCurrentAnimatorStateInfo(0).IsName("HeavyAttack")) player.anim.Play("Idle");
        StickToGround(true);
    }

    public void StickToGround(bool a) //Allow or disallow every functionality
    {
        CanAttack = a;
        player.movementScript.CanMove = a;
        player.movementScript.RotateSlow = !a;
        player.movementScript.CanDash = a;
        player.movementScript.CanJump = a;
        CanBlock = a;
    }

    void ResetBlock()
    {
        player.anim.ResetTrigger("StopBlock");
        CanBlock = true;
    }

    public void InterruptCharging()
    {
        IsCharging = false;
        CanAttack = true;
    }


    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = new Color32(255, 0, 0, 200);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, maxDistance);
    }
}
