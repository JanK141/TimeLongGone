using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "Player Variables", menuName = "Variable/Player")]
    public class PlayerVariables : ScriptableObject
    {
        [Header("Movement")]
            [Tooltip("Vertical velocity added per frame")]public float initialGravity;
            [Tooltip("Speed in normal conditions")]public float normalSpeed;
            [Tooltip("Speed while blocking/charging")] public float slowSpeed;
            [Tooltip("Time used for interpolating LookAt while slow rotating (e.g. while attacking). Lower number = faster rotation")]public float rotationTime;
            public float jumpHeight;

            [Space]

        [Header("Dashing")]
            public float dashCooldown;
            public float dashDistance;
            public float dashTime;
            public float iframesTime;
            [Tooltip("Max distance that fully charged dash attack can travel")]public float dashAttackMaxDist;

            [Space]

        [Header("Attacking")]
            public float baseDamage;
            public float attackCooldown;
            [Tooltip("Radius of attack hitbox")]public float attackRadius;
            [Tooltip("How far from player center of attack hitbox is set")]public float attackDistance;
            [Tooltip("For how long after performing attack animation can chain into next attack animation")]public float chainTime;
            [Tooltip("For how long player negates gravity while performing an attack")]public float airborneTime;
            [Tooltip("Maximum multiplier that can be applied on base dmg while performing dash attack")]public float maxChargeMult;

            [Space]

        [Header("Blocking")]
            public float blockCooldown;
            [Tooltip("For how long after pressing block hit is treated as parried")]public float parryWindow;
            [Tooltip("For how long collision with enemy weapon should be ignored after receiving hit")]public float postHitNoCollision;
            [Tooltip("Knockback velocity after getting hit")]public float pushVelocity;
            [Tooltip("How long knockback should apply velocity")]public float pushTime;
        
            [Space]
        
        [Header("Combo")]
            [Tooltip("What portion of base damage should be added to output damage witch each combo multiplier")]public float comboMultiplier;
            [Tooltip("Time for combo to expire")]public float comboTimeout;
            [Tooltip("Multiplier of base damage for finisher attack")]public float finisherMultiplier;

            [Space]
            
        [Header("Time control")] 
            [Tooltip("How much faster player gets when time is slowed down")][Range(0,1)]public float speedBoost;
            [Tooltip("Total pool of mana")]public float mana;
            [Tooltip("How much mana is beeing regenerated per second")]public float manaRegPerSecond;
            [Tooltip("How much mana is beeing rewarded for successful parry/dodge/stun")]public float manaReward;
            [Tooltip("Amount drained per second of slow motion")]public float slowmoCostPerSecond;
            [Tooltip("Amount needed to start rewinding time")]public float rewindFlatCost;
            [Tooltip("Amount drained per second of rewinding")]public float rewindCostPerSecond;
    }
}