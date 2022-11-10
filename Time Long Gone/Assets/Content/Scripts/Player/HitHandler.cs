using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Enemy;
using Content.Scripts.Player;
using Content.Scripts.tim;
using UnityEngine;

public class HitHandler : MonoBehaviour
{
    [SerializeField] private float parryWindow = 0.5f;
    [SerializeField] private float noCollisionTime = 0.2f;

    private PlayerScript player;

    void Start()
    {
        player = PlayerScript.Instance;
    }

    public void ReceiveHit()
    {
        print("HIT " + EnemyStatusScript.currStatus);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("EnemyWeapon"), true);
        Invoke(nameof(ResetCollision), noCollisionTime);
        switch (EnemyStatusScript.currStatus)
        {
            case Statuses.Regular:
                if(player.movementScript.IsInvincible) return;
                if (player.combat.IsBlocking)
                {
                    if (Time.time - player.combat.BlockPressTime <= parryWindow)
                    {
                        EnemyScript.Instance.ReceiveParry();
                        return;
                    }
                }
                break;
            case Statuses.Unblockable:
                if (player.movementScript.IsInvincible) return;
                if (player.combat.IsBlocking && (Time.time - player.combat.BlockPressTime <= parryWindow))
                {
                    EnemyScript.Instance.ReceiveParry();
                    return;
                }
                break;
            case Statuses.Unavoidable:
                break;
        }
        //Death

        StartCoroutine(Controller.Instance.PlayerDead());
        player.IsAlive = false;
    }

    void ResetCollision() => Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("EnemyWeapon"), false);
}
