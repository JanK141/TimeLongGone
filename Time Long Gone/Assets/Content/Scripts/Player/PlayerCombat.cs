using System.Collections;
using Content.Scripts.Player;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    #region Inspector Fields

    [Header("Normal attack")] [SerializeField]
    float attackRadius = 1;

    [SerializeField] private float attackDistance = 0.5f;

    [SerializeField] private float damage = 10f;
    //[SerializeField] private float attackCooldown = 0.2f;

    [Header("Charged attack")] [SerializeField]
    private Collider chargedAttackHitBox;

    [SerializeField] private float minClampPower = 1.5f;
    [SerializeField] private float maxClampPower = 3f;

    [Space] [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Animator anim;

    #endregion

    #region Cached dependencies

    private PlayerScript player;
    private ChargedAttackTrigger chargedHitBox;
    private CharacterController controller;

    #endregion

    #region private variables

    private bool canAttack = true;

    #endregion

    private void Start()
    {
        player = PlayerScript.Instance;
        chargedHitBox = chargedAttackHitBox.GetComponent<ChargedAttackTrigger>();
        controller = GetComponent<CharacterController>();
    }

    public void Attack()
    {
        if (!canAttack) return;
        anim.SetTrigger("Attack");
        if (!Physics.CheckSphere
            (transform.position + transform.forward * attackDistance, attackRadius,
                enemyMask)
           ) return;
        DummyTest.Instance.Damage(damage);
        EnemyHealth.Instance.CurrHealth -= damage;
    }

    public void ChargedAttack(float power)
    {
        if (!canAttack) return;
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
        var motion = transform.forward;
        var time = 0f;

        while (time < player.movementScript.DashTime)
        {
            if (!pm.isInvincible && time >= pm.FramesStart * pm.DashTime) pm.isInvincible = true;
            if (pm.isInvincible && time >= pm.FramesEnd * pm.DashTime) pm.isInvincible = false;
            time += Time.deltaTime;
            controller.Move(motion * (pm.Speed * strength * Time.deltaTime));
            yield return null;
        }

        pm.CanDash = true;
        pm.CanMove = true;
        Physics.IgnoreCollision(controller, DummyTest.Instance.GetComponent<Collider>(), false);
        chargedHitBox.gameObject.SetActive(false);
    }
}