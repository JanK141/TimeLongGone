using System.Collections;
using Content.Scripts.Enemy;
using Content.Scripts.Player;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    #region Inspector Fields

    [Header("Normal attack")] [SerializeField]
    float attackRadius = 1;

    [SerializeField] private float attackDistance = 0.5f;
    [SerializeField] private int damage = 10;

    [Header("Combo")] 
    [SerializeField] private float timeToChain = 0.8f;
    [SerializeField] private float comboDamageMult = 0.1f;
    [SerializeField] public float comboTimeout = 4f;
    [SerializeField] private int comboMultCap = 20;

    [Header("Charged attack")] [SerializeField]
    private Collider chargedAttackHitBox;

    [SerializeField] private float minClampPower = 1.5f;
    [SerializeField] private float maxClampPower = 3f;
    [SerializeField] private float maxDistance = 8f;

    [Header("Block")] [SerializeField] private float blockCD = 0.5f;

    [Space] 
    [SerializeField] Collider StunAttackHitbox;
    [SerializeField] private LayerMask enemyMask;
    #endregion

    #region Cached dependencies

    private PlayerScript player;
    private ChargedAttackTrigger chargedHitBox;
    private CharacterController controller;
    private float _timeout = 0f;
    private float _lastAttack = 0f;
    private int _attackInCombo = 0;
    private static readonly int StopBlock = Animator.StringToHash("StopBlock");

    #endregion

    #region Properties
    public bool CanAttack { get; set; } = true;
    public int Damage { get; set; }
    public bool IsCharging { get; set; } = false;
    public bool IsBlocking { get; set; } = false;
    public bool CanBlock { get; set; } = true;
    public int Combo { get; set; } = 0;
    public float BlockPressTime { get; set; } = 0;
    #endregion

    private void Start()
    {
        Damage = damage;
        player = PlayerScript.Instance;
        chargedHitBox = chargedAttackHitBox.GetComponent<ChargedAttackTrigger>();
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Combo <= 0) return;
        
        _timeout -= Time.deltaTime;
        
        if (!(_timeout <= 0)) return;
        
        Combo = 0;
        player.InvokeCombo(Combo);
    }

    public void Attack()
    {
        if(!CanAttack) return;

        CanAttack = false;
        player.movementScript.canRotate = false;
        player.movementScript.canMove = false;
        if (Time.time - _lastAttack > timeToChain) _attackInCombo = 0;
        _attackInCombo++;
        _lastAttack = Time.time;

        switch (_attackInCombo)
        {
            case 1:
                Damage = (int)(damage + damage * comboDamageMult * Mathf.Min(Combo, comboMultCap));
                player.anim.Play("Attack1");
                if (!player.movementScript.isGrounded) StartCoroutine(player.movementScript.StopInAir(0.5f));
                break;
            case 2:
                Damage = (int)(damage + damage * comboDamageMult * Mathf.Min(Combo, comboMultCap));
                player.anim.Play("Attack2");
                StopCoroutine(player.movementScript.StopInAir(1));
                if (!player.movementScript.isGrounded) StartCoroutine(player.movementScript.StopInAir(0.5f));
                break;
            case 3:
                Damage = (int)(1.5f * damage + damage * comboDamageMult * Mathf.Min(Combo, comboMultCap));
                player.anim.Play("Attack3");
                StopCoroutine(player.movementScript.StopInAir(1));
                if (!player.movementScript.isGrounded) StartCoroutine(player.movementScript.StopInAir(0.1f));
                _attackInCombo = 0;
                break;
        }

    }

    public void Hit(bool isFinisher)
    {
        if (!Physics.CheckSphere(transform.position + transform.forward * attackDistance, attackRadius,
                enemyMask)) return;
        
        if (isFinisher) ContinueCombo(-1);
        else ContinueCombo(1);

        EnemyScript.Instance.ReceiveHit(Damage);
    }

    public void HeavyAttack()
    {
        if(!CanAttack || !player.movementScript.isGrounded) return;
        
        player.anim.Play("HeavyAttack");
        StickToGround(false);
        Damage = (int)(damage + damage * comboDamageMult* 2 * Combo);
    }

    public void StunAttack()
    {
        StickToGround(false);
        player.anim.Play("StunAttack");
        StunAttackHitbox.gameObject.SetActive(true);
        Invoke(nameof(ResetStunAttack), 0.4f);
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
            BlockPressTime = Time.time;
        }
        else
        {
            CanBlock = false;
            player.anim.SetTrigger(StopBlock);
            Invoke(nameof(ResetBlock), blockCD);
        }
    }

    public void StartCharging()
    {
        if (!CanAttack || !player.movementScript.isGrounded) return;
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
        chargedHitBox.damage = (int)((damage + damage* comboDamageMult*0.5f * Mathf.Min(Combo, comboMultCap)) * strength);
        Physics.IgnoreCollision(controller, EnemyScript.Instance.GetComponent<Collider>(), true);

        yield return StartCoroutine(player.movementScript.Dash(Mathf.Clamp((strength-minClampPower) / (maxClampPower - minClampPower),0.5f, 1f) * maxDistance));

        Physics.IgnoreCollision(controller, EnemyScript.Instance.GetComponent<Collider>(), false);
        chargedHitBox.gameObject.SetActive(false);
        player.movementScript.ResetSpeed();
    }

    public void InvokeAttackReset(float time) => Invoke(nameof(AttackReset), time);
    public void AttackReset()
    {
        CanAttack = true;
        player.movementScript.canRotate = true;
        player.movementScript.canMove = true;
    }

    public void ResetHeavy()
    {
        if (player.anim.GetCurrentAnimatorStateInfo(0).IsName("HeavyAttack")) player.anim.Play("Idle");
        StickToGround(true);
    }

    public void StickToGround(bool a) //Allow or disallow every functionality
    {
        CanAttack = a;
        player.movementScript.canMove = a;
        player.movementScript.rotateSlow = !a;
        player.movementScript.canDash = a;
        player.movementScript.canJump = a;
        CanBlock = a;
    }

    void ResetBlock()
    {
        player.anim.ResetTrigger(StopBlock);
        CanBlock = true;
    }

    private void ResetStunAttack() => StunAttackHitbox.gameObject.SetActive(false);

    public void InterruptCharging()
    {
        IsCharging = false;
        CanAttack = true;
    }

    public void ContinueCombo(int value)
    {
        if (value < 0) _timeout = 0;
        else
        {
            _timeout = comboTimeout;
            Combo += value;
            player.InvokeCombo(Combo);
        }
    }

    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = new Color32(255, 0, 0, 200);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, maxDistance);
    }
}